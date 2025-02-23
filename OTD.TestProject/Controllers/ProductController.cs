using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OTD.Core.Models.Responses;
using OTD.Core.Models.SearchRequests;
using OTD.Repository.Abstract;
using OTD.ServiceLayer.Abstract;
using OTD.ServiceLayer.Concrete;

namespace OTD.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _service;

        public ProductController(IProductRepository repository, IMapper mapper, ICacheService cache)
        {
            _service = new ProductService(repository, mapper, cache);
        }

        [HttpGet("GetProducts")]
        public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetProducts([FromQuery] int? category)
        {
            var response = await _service.List(new ProductSearchRequest() { Category = category });
            return Ok(response);
        }
    }
}
