using OTD.Core.Entities;
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

        public UserService(IUserRepository repository, IMailService mailService)
        {
            _repository = repository;
            _mailService = mailService;
        }

        public async Task<ApiResponse> Register(RegisterRequest request)
        {
            var userAlreadyExists = (await _repository.List(p => p.Email == request.Email)).FirstOrDefault();
            if (userAlreadyExists != null)
                return GenerateResponse<ApiResponse>(false, ErrorCode.EmailAlreadyInUse, null);

            var otpCode = GenerateOtp();
            var user = new User
            {
                UserId = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                EmailConfirmationCode = otpCode,
                EmailConfirmationExpireDate = DateTime.UtcNow.AddMinutes(10),
                IsEmailConfirmed = false
            };

            await _repository.Add(user);

            await _mailService.SendEmailAsync(user.Email, "Email Confirmation Code", $"One time password: {otpCode}");

            return GenerateResponse(true, ErrorCode.Success, user);
        }

        public async Task<ApiResponse> ConfirmEmail(ConfirmEmailRequest request)
        {
            var user = (await _repository.List(p => p.Email == request.Email)).FirstOrDefault();

            if (user == null) 
                return GenerateResponse<ApiResponse>(false, ErrorCode.NotFound, null);

            if (user.IsEmailConfirmed)
                return GenerateResponse<ApiResponse>(false, ErrorCode.EmailAlreadyConfirmed, null);

            if (user.EmailConfirmationExpireDate < DateTime.UtcNow)
                return GenerateResponse<ApiResponse>(false, ErrorCode.OtpExpired, null);

            user.IsEmailConfirmed = true;
            user.EmailConfirmationCode = null;
            user.EmailConfirmationExpireDate = null;

            await _repository.Update(user);

            return GenerateResponse(true, ErrorCode.Success, user);
        }

        //TODO:
        public async Task<ApiResponse> Login(LoginRequest request)
        {
            throw new NotImplementedException();
        }

        //TODO:
        public async Task<ApiResponse> ResendOtp(ResendOtpRequest request)
        {
            throw new NotImplementedException();
        }

        private string GenerateOtp()
        {
            Random random = new Random();
            return random.Next(100000,999999).ToString();
        }
    }
}
