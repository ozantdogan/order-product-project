using OTD.Core.Models.Requests;
using OTD.Core.Models.Responses;

namespace OTD.ServiceLayer.Abstract
{
    public interface IOrderService
    {
        Task<ApiResponse> Add(CreateOrderRequest request);
    }
}
