using OTD.Core.Models.Requests;
using OTD.Core.Models.Responses;
using OTD.Core.Models.SearchRequests;

namespace OTD.ServiceLayer.Abstract
{
    public interface IOrderService
    {
        Task<ApiResponse> Add(CreateOrderRequest request);
        Task<ApiResponse<List<OrderResponse>>> List(OrderSearchRequest request);
    }
}
