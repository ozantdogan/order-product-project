using OTD.ServiceLayer.Abstract;
using OTD.ServiceLayer.Helper;
using OTD.Core.Entities;
using OTD.Core.Models.Requests;
using OTD.Core.Models.Responses;
using OTD.Repository.Abstract;

namespace OTD.ServiceLayer.Concrete
{
    public class OrderService : BaseService, IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IRabbitMqService _rabbitMqService;

        public OrderService(IOrderRepository repository, IOrderDetailRepository orderDetailRepository, IRabbitMqService rabbitMqService)
        {
            _repository = repository;
            _orderDetailRepository = orderDetailRepository;
            _rabbitMqService = rabbitMqService;
        }

        public async Task<ApiResponse> Add(CreateOrderRequest request)
        {
            var order = new Order()
            {
                OrderId = Guid.NewGuid(),
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                CustomerGSM = request.CustomerGSM
            };

            var totalAmount = 0;

            var products = request.ProductDetails;
            if (products == null || !products.Any())
                return GenerateResponse<ApiResponse>(false, ErrorCode.Failed, null);

            var orderDetails = new List<OrderDetail>();
            foreach(var product in products)
            {
                var orderDetail = new OrderDetail()
                {
                    OrderId = order.OrderId,
                    ProductId = product.ProductId,
                    UnitPrice = product.UnitPrice
                };

                totalAmount += product.Amount;
                orderDetails.Add(orderDetail);
            }

            order.TotalAmount = totalAmount;

            await _repository.Add(order);
            await _orderDetailRepository.AddRange(orderDetails);

            var mailRequest = new MailRequest
            {
                To = order.CustomerEmail,
                Subject = "Order Confirmation",
                Body = $"Dear {order.CustomerName}, your order has been successfully created."
            };

            _rabbitMqService.Publish("SendMail", mailRequest);

            return GenerateResponse(true, ErrorCode.Success, request);
        }
    }
}
