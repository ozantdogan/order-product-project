using OTD.ServiceLayer.Helper;
using OTD.Core.Models.Responses;
using OTD.ServiceLayer.Abstract;

namespace OTD.ServiceLayer.Concrete
{
    public class BaseService : IBaseService
    {
        public ApiResponse<T> GenerateResponse<T>(bool success, ErrorCode code, T? data)
        {
            return new ApiResponse<T>
            {
                Success = success,
                ResultMessage = code.Message,
                Data = data
            };
        }
    }
}
