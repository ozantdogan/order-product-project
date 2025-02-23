using OTD.Core.Models.Responses;
using OTD.ServiceLayer.Helper;

namespace OTD.ServiceLayer.Abstract
{
    public interface IBaseService
    {
        ApiResponse<T> GenerateResponse<T>(bool success, ErrorCode code, T? data);
    }
}

