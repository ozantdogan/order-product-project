using OTD.Core.Models.Responses;
using OTD.Core.Models.SearchRequests;

namespace OTD.ServiceLayer.Abstract
{
    public interface IProductService
    {
        Task<ApiResponse<List<ProductDto>>> List(ProductSearchRequest request);
    }
}
