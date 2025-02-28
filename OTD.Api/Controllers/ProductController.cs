using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OTD.Core.Models.Responses;
using OTD.Core.Models.SearchRequests;
using OTD.ServiceLayer.Abstract;

namespace OTD.Api.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        [HttpGet("GetProducts")]
        public async Task<ActionResult<ApiResponse<List<ProductResponse>>>> GetProducts([FromQuery] ProductSearchRequest request)
        {
            var response = await _service.List(request);
            return Ok(response);
        }
    }
}
