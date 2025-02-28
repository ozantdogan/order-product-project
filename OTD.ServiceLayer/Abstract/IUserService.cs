using OTD.Core.Models.Requests;
using OTD.Core.Models.Responses;

namespace OTD.ServiceLayer.Abstract
{
    public interface IUserService
    {
        Task<ApiResponse> Register(RegisterRequest model);
        Task<ApiResponse> ConfirmEmail(ConfirmEmailRequest model);
        Task<ApiResponse> ResendConfirmationCode(ResendConfirmationCodeRequest model);
        Task<ApiResponse> Login(LoginRequest model);
        Task<ApiResponse> RefreshToken(RefreshTokenRequest model);
    }

}
