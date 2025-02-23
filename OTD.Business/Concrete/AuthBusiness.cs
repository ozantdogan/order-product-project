using OTD.ServiceLayer.Abstract;
using OTD.Core.Helper;
using OTD.Core.Models;
using OTD.Core.Models.Requests;

namespace OTD.ServiceLayer.Concrete
{
    public class AuthBusiness : IAuthBusiness
    {
        private readonly ITokenBusiness tokenBusiness;
        public AuthBusiness(ITokenBusiness tokenBusiness)
        {
            this.tokenBusiness = tokenBusiness;
        }

        public async Task<UserLoginResponse> LoginUserAsync(UserLoginRequest request)
        {
            UserLoginResponse response = new();

            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Username == "Test" && request.Password == "123")
            {
                var generatedTokenInfo = await tokenBusiness.GenerateToken(new GenerateTokenRequest { Username = request.Username });

                response.AuthenticateResult = true;
                response.AuthToken = generatedTokenInfo.Token;
                response.AccessTokenExpireDate = generatedTokenInfo.TokenExpireDate;
            }

            return response;
        }
    }
}
