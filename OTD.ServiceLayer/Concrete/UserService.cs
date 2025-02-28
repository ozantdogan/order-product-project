using AutoMapper;
using Microsoft.Extensions.Configuration;
using OTD.Core.Entities;
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
        private readonly IRabbitMqService _rabbitMqService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public UserService(IUserRepository repository,
            IRabbitMqService rabbitMqService,
            IConfiguration configuration,
            IMapper mapper,
            ICacheService cache)
        {
            _repository = repository;
            _rabbitMqService = rabbitMqService;
            _configuration = configuration;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ApiResponse> Register(RegisterRequest request)
        {
            var user = await _cache.GetAsync<User>($"{nameof(User)}-{request.Email}");
            if (user == null)
                user = (await _repository.List(p => p.Email == request.Email)).FirstOrDefault();

            if (user != null)
                return GenerateResponse<ApiResponse>(false, ErrorCode.EmailAlreadyInUse, null);

            var confirmationCode = GenerateConfirmationCode();
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            await _cache.SetCacheAsync($"confirmation-code:{request.Email}", confirmationCode, 10);

            var newUser = new User
            {
                UserId = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                DisplayName = request.FirstName + " " + request.LastName,
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                IsEmailConfirmed = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = null,
            };

            await _repository.Add(newUser);

            var response = _mapper.Map<UserResponse>(newUser);

            var mailRequest = new MailRequest
            {
                To = newUser.Email,
                Subject = $"Your email confirmation code",
                Body = $"{confirmationCode}"
            };

            _rabbitMqService.Publish("SendMail", mailRequest);

            return GenerateResponse(true, ErrorCode.Success, response);
        }

        public async Task<ApiResponse> ConfirmEmail(ConfirmEmailRequest request)
        {
            string cacheKey = $"confirmation-code:{request.Email}";
            var cacheResult = await _cache.GetAsync<string>(cacheKey);
            if (cacheResult == null || cacheResult != request.ConfirmationCode)
                return GenerateResponse<ApiResponse>(false, ErrorCode.ConfirmationCodeNotValid, null);

            var user = await _cache.GetAsync<User>($"{nameof(User)}-{request.Email}");
            if (user == null)
                user = (await _repository.List(p => p.Email == request.Email)).FirstOrDefault();

            if (user == null)
                return GenerateResponse<ApiResponse>(false, ErrorCode.NotFound, null);

            if (user.IsEmailConfirmed)
                return GenerateResponse<ApiResponse>(false, ErrorCode.EmailAlreadyConfirmed, null);

            user.IsEmailConfirmed = true;
            await _repository.Update(user);

            await _cache.RemoveCacheAsync($"confirmation-code:{request.Email}");

            var response = _mapper.Map<UserResponse>(user);

            return GenerateResponse(true, ErrorCode.Success, response);
        }

        public async Task<ApiResponse> Login(LoginRequest request)
        {
            var user = await _cache.GetAsync<User>($"{nameof(User)}-{request.Email}");
            if (user == null)
                user = (await _repository.List(p => p.Email == request.Email)).FirstOrDefault();

            if (user == null)
                return GenerateResponse<ApiResponse>(false, ErrorCode.EmailOrPasswordNotCorrect, null);

            if (!VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
                return GenerateResponse<ApiResponse>(false, ErrorCode.EmailOrPasswordNotCorrect, null);

            if (!user.IsEmailConfirmed)
                return GenerateResponse<ApiResponse>(false, ErrorCode.EmailNotConfirmed, null);

            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            var cacheKey = $"{nameof(User)}-{user.Email}";
            await _cache.SetCacheAsync(cacheKey, user, 60);
            await _cache.SetCacheAsync($"refresh-token:{refreshToken}", user.UserId, 7);

            var response = new LoginResponse()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
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

        public async Task<ApiResponse> ResendConfirmationCode(ResendConfirmationCodeRequest request)
        {
            var user = await _cache.GetAsync<User>($"{nameof(User)}-{request.Email}");
            if (user == null)
                user = (await _repository.List(p => p.Email == request.Email)).FirstOrDefault();

            if (user == null)
                return GenerateResponse<ApiResponse>(false, ErrorCode.EmailOrPasswordNotCorrect, null);

            if (user.IsEmailConfirmed)
                return GenerateResponse<ApiResponse>(false, ErrorCode.EmailAlreadyConfirmed, null);

            var confirmationCode = GenerateConfirmationCode();

            await _cache.SetCacheAsync($"confirmation-code:{request.Email}", confirmationCode, 10);

            await _repository.Update(user);

            var mailRequest = new MailRequest
            {
                To = user.Email,
                Subject = $"Your email confirmation code",
                Body = $"{confirmationCode}"
            };

            _rabbitMqService.Publish("SendMail", mailRequest);

            return GenerateResponse<ApiResponse>(true, ErrorCode.Success, null);
        }

        public async Task<ApiResponse> RefreshToken(RefreshTokenRequest request)
        {
            var isBlacklisted = await _cache.GetAsync<bool>($"blacklisted-refresh-token:{request.RefreshToken}");
            if (isBlacklisted)
                return GenerateResponse<ApiResponse>(false, ErrorCode.ExpiredRefreshToken, null);

            var userId = await _cache.GetAsync<Guid?>($"refresh-token:{request.RefreshToken}");
            if (userId == null)
                return GenerateResponse<ApiResponse>(false, ErrorCode.ExpiredRefreshToken, null);

            var user = (await _repository.List(p => p.UserId == userId.Value)).FirstOrDefault();
            if (user == null)
                return GenerateResponse<ApiResponse>(false, ErrorCode.NotFound, null);

            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            await _cache.SetCacheAsync($"blacklisted-refresh-token:{request.RefreshToken}", true, 7);
            await _cache.SetCacheAsync($"refresh-token:{newRefreshToken}", user.UserId, 7);

            var response = new RefreshTokenResponse()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };

            return GenerateResponse(true, ErrorCode.Success, response);
        }

        private string GenerateConfirmationCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
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

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }

    }
}
