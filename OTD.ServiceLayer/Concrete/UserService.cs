using AutoMapper;
using Microsoft.Extensions.Configuration;
using OTD.Core.Entities;
using OTD.Core.Helpers;
using OTD.Core.Models;
using OTD.Core.Models.Requests;
using OTD.Core.Models.Responses;
using OTD.Repository.Abstract;
using OTD.ServiceLayer.Abstract;
using OTD.ServiceLayer.Helper;

namespace OTD.ServiceLayer.Concrete
{
    public class UserService : BaseService, IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository, IMailService mailService, IConfiguration configuration, IMapper mapper)
        {
            _repository = repository;
            _mailService = mailService;
            _configuration = configuration;
            _mapper = mapper;   
        }

        public async Task<ApiResponse> Register(RegisterRequest request)
        {
            var userAlreadyExists = (await _repository.List(p => p.Email == request.Email)).FirstOrDefault();
            if (userAlreadyExists != null)
                return GenerateResponse<ApiResponse>(false, ErrorCode.EmailAlreadyInUse, null);

            var emailFormatValidation = ValidationHelper.ValidateEmailFormat(request.Email);
            if(!emailFormatValidation)
                return GenerateResponse<ApiResponse>(false, ErrorCode.EmailFormatValidationFailed, null);

            var otpCode = GenerateOtp();
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                UserId = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                DisplayName = request.FirstName + " " + request.LastName,
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                EmailConfirmationCode = otpCode,
                EmailConfirmationExpireDate = DateTime.UtcNow.AddMinutes(10),
                IsEmailConfirmed = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = null,
            };

            await _repository.Add(user);

            var response = _mapper.Map<UserResponse>(user);

            await _mailService.SendEmailAsync(user.Email, "Email Confirmation Code", $"One time password: {otpCode}");

            return GenerateResponse(true, ErrorCode.Success, response);
        }

        public async Task<ApiResponse> ConfirmEmail(ConfirmEmailRequest request)
        {
            var user = (await _repository.List(p => p.Email == request.Email)).FirstOrDefault();

            if (user == null) 
                return GenerateResponse<ApiResponse>(false, ErrorCode.NotFound, null);

            if (user.IsEmailConfirmed)
                return GenerateResponse<ApiResponse>(false, ErrorCode.EmailAlreadyConfirmed, null);

            if (user.EmailConfirmationExpireDate < DateTime.UtcNow || user.EmailConfirmationCode != request.Otp)
                return GenerateResponse<ApiResponse>(false, ErrorCode.OtpNotValid, null);

            user.IsEmailConfirmed = true;
            user.EmailConfirmationCode = null;
            user.EmailConfirmationExpireDate = null;

            await _repository.Update(user);

            return GenerateResponse(true, ErrorCode.Success, user);
        }

        public async Task<ApiResponse> Login(LoginRequest request)
        {
            var emailFormatValidation = ValidationHelper.ValidateEmailFormat(request.Email);
            if(!emailFormatValidation)
                return GenerateResponse<ApiResponse>(false, ErrorCode.EmailFormatValidationFailed, null);

            var user = (await _repository.List(p => p.Email == request.Email)).FirstOrDefault();
            if (user == null)
                return GenerateResponse<ApiResponse>(false, ErrorCode.InvalidCredentials, null);

            if (!VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
                return GenerateResponse<ApiResponse>(false, ErrorCode.InvalidCredentials, null);

            if (!user.IsEmailConfirmed)
                return GenerateResponse<ApiResponse>(false, ErrorCode.EmailNotConfirmed, null);

            var token = GenerateJwtToken(user);
            var response = new LoginResponse()
            {
                AuthToken = token,
                AccessTokenExpireDate = DateTime.UtcNow.AddHours(1),
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            user.LastLoginDate = DateTime.UtcNow;
            await _repository.Update(user);

            return GenerateResponse(true, ErrorCode.Success, response);
        }

        public async Task<ApiResponse> ResendOtp(ResendOtpRequest request)
        {
            var emailFormatValidation = ValidationHelper.ValidateEmailFormat(request.Email);
            if (!emailFormatValidation)
                return GenerateResponse<ApiResponse>(false, ErrorCode.EmailFormatValidationFailed, null);

            var user = (await _repository.List(p => p.Email == request.Email)).FirstOrDefault();

            if (user == null)
                return GenerateResponse<ApiResponse>(false, ErrorCode.InvalidCredentials, null);

            if (user.IsEmailConfirmed)
                return GenerateResponse<ApiResponse>(false, ErrorCode.EmailAlreadyConfirmed, null);

            var otpCode = GenerateOtp();
            user.EmailConfirmationCode = otpCode;
            user.EmailConfirmationExpireDate = DateTime.UtcNow.AddMinutes(10);

            await _repository.Update(user);
            await _mailService.SendEmailAsync(user.Email, "Resend Email Confirmation Code", $"New OTP: {otpCode}");

            return GenerateResponse<ApiResponse>(true, ErrorCode.Success, null);
        }

        private string GenerateOtp()
        {
            Random random = new Random();
            return random.Next(100000,999999).ToString();
        }

        private bool VerifyPassword(string enteredPassword, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(enteredPassword));
                return computedHash.SequenceEqual(storedHash);
            }
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var secretKey = _configuration["JwtSettings:SecretKey"];
            var key = System.Text.Encoding.UTF8.GetBytes(secretKey ?? "YOUR-VERY-SECURE-SECRET-KEY");
            var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                    new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                    Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
