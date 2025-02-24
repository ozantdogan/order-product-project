using OTD.ServiceLayer.Abstract;
using OTD.ServiceLayer.Helper;
using OTD.Core.Entities;
using OTD.Core.Models.Requests;
using OTD.Core.Models.Responses;
using OTD.Repository.Abstract;
using OTD.Core.Models.SearchRequests;
using AutoMapper;
using System.Text.Json;
using System.Linq.Expressions;

namespace OTD.ServiceLayer.Concrete
{
    public class OrderService : BaseService, IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IProductRepository _productRepository;
        private readonly IRabbitMqService _rabbitMqService;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public OrderService(IOrderRepository repository, 
            IOrderDetailRepository orderDetailRepository, 
            IProductRepository productRepository, 
            IRabbitMqService rabbitMqService, 
            IMapper mapper,
            ICacheService cache)
        {
            _repository = repository;
            _orderDetailRepository = orderDetailRepository;
            _productRepository = productRepository;
            _rabbitMqService = rabbitMqService;
            _mapper = mapper;
            _cache = cache;
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
            var requestedProductIds = products.Select(p => p.ProductId).ToList();
            var existingProducts = await _productRepository.List(p => requestedProductIds.Contains(p.ProductId));
            var existingProductIds = existingProducts.Select(p => p.ProductId).ToHashSet();
            var missingProductIds = requestedProductIds.Where(id => !existingProductIds.Contains(id)).ToList();
            if (missingProductIds.Any())
                return GenerateResponse(false, ErrorCode.ProductNotFound, missingProductIds);

            var orderDetails = new List<OrderDetail>();
            foreach (var product in products)
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

        public async Task<ApiResponse<List<OrderResponse>>> List(OrderSearchRequest request)
        {
            string cacheKey = $"{nameof(Order)}-{JsonSerializer.Serialize(request)}";
            var cacheResult = await _cache.GetAsync<List<OrderResponse>>(cacheKey);
            if (cacheResult is List<OrderResponse> && cacheResult != null)
                return GenerateResponse(true, ErrorCode.Success, cacheResult);

            var predicate = GetExpression(request);
            var data = await _repository.List(predicate);
            var dto = _mapper.Map<List<OrderResponse>>(data);  

            await _cache.SetCacheAsync(cacheKey, dto, 30);

            return GenerateResponse(true, ErrorCode.Success, dto);
        }

        private Expression<Func<Order, bool>> GetExpression(OrderSearchRequest request)
        {
            var predicate = PredicateBuilder.True<Order>();

            if (request.CreatedOnMin.HasValue)
                predicate = predicate.And(p => p.CreatedOn >= request.CreatedOnMin);

            if (request.CreatedOnMax.HasValue)
                predicate = predicate.And(p => p.CreatedOn <= request.CreatedOnMax);

            if (!string.IsNullOrWhiteSpace(request.CustomerGSM))
                predicate = predicate.And(p => p.CustomerGSM.Contains(request.CustomerGSM));

            if (!string.IsNullOrWhiteSpace(request.CustomerName))
                predicate = predicate.And(p => p.CustomerName.Contains(request.CustomerName));

            if (!string.IsNullOrWhiteSpace(request.CustomerEmail))
                predicate = predicate.And(p => p.CustomerEmail.Contains(request.CustomerEmail));

            if (request.TotalAmountMin.HasValue)
                predicate = predicate.And(p => p.TotalAmount >= request.TotalAmountMin);

            if (request.TotalAmountMax.HasValue)
                predicate = predicate.And(p => p.TotalAmount <= request.TotalAmountMax);

            return predicate;
        }
    }
}
