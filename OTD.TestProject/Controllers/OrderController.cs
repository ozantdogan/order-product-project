using Microsoft.AspNetCore.Mvc;
using OTD.Core.Models.Requests;
using OTD.Core.Models.Responses;
using OTD.ServiceLayer.Abstract;

namespace OTD.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrderController(IOrderService service)
        {
            _service = service;
        }

        [HttpPost("CreateOrder")]
        public async Task<ActionResult<ApiResponse>> CreateOrder(CreateOrderRequest request)
        {
            var response = await _service.Add(request);
            return Ok(response);
        }
    }
}
