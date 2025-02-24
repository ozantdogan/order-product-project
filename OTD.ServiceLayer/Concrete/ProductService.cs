using OTD.ServiceLayer.Abstract;
using OTD.Core.Models.Responses;
using OTD.Repository.Abstract;
using AutoMapper;
using OTD.Core.Models.SearchRequests;
using OTD.Core.Entities;
using OTD.ServiceLayer.Helper;
using System.Linq.Expressions;
using System.Text.Json;

namespace OTD.ServiceLayer.Concrete
{
    public class ProductService : BaseService, IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public ProductService(IProductRepository repository, IMapper mapper, ICacheService cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ApiResponse<List<ProductResponse>>> List(ProductSearchRequest request)
        {
            string cacheKey = $"{nameof(Product)}-{JsonSerializer.Serialize(request)}";
            var cacheResult = await _cache.GetAsync<List<ProductResponse>>(cacheKey);
            if (cacheResult is List<ProductResponse> && cacheResult != null)
                return GenerateResponse(true, ErrorCode.Success, cacheResult);

            var predicate = GetExpression(request);
            var data = await _repository.List(predicate);
            var dto = _mapper.Map<List<ProductResponse>>(data);

            await _cache.SetCacheAsync(cacheKey, dto, 30);

            return GenerateResponse(true, ErrorCode.Success, dto);
        }

        private Expression<Func<Product, bool>> GetExpression(ProductSearchRequest request)
        {
            var predicate = PredicateBuilder.True<Product>();

            if (request.Category.HasValue)
                predicate = predicate.And(p => (int)p.Category == request.Category);

            return predicate;
        }
    }
}
