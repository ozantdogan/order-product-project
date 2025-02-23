using OTD.ServiceLayer.Helper;
using OTD.Core.Models.Responses;

namespace OTD.ServiceLayer.Abstract
{
    public class BaseService
    {
        internal ApiResponse<T> GenerateResponse<T>(bool success, ErrorCode code, T? data)
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
