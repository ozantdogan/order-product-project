using OTD.Core.Models;
using OTD.Core.Models.Requests;

namespace OTD.ServiceLayer.Abstract
{
    public interface IAuthBusiness
    {
        public Task<UserLoginResponse> LoginUserAsync(UserLoginRequest request);
    }
}