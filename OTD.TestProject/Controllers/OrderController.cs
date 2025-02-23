using Microsoft.AspNetCore.Mvc;
using OTD.ServiceLayer.Concrete;
using OTD.Core.Models.Requests;
using OTD.Core.Models.Responses;
using OTD.Repository.Abstract;

namespace OTD.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _service;
        public OrderController(IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository)
        {
            _service = new OrderService(orderRepository, orderDetailRepository);
        }

        [HttpPost("CreateOrder")]
        public async Task<ActionResult<ApiResponse>> CreateOrder(CreateOrderRequest request)
        {
            var response = await _service.Add(request);
            return Ok(response);
        }
    }
}
