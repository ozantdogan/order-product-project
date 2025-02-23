using OTD.Core.Helper;

namespace OTD.ServiceLayer.Abstract
{
    public interface ITokenBusiness
    {
        public Task<GenerateTokenResponse> GenerateToken(GenerateTokenRequest request);
    }
}