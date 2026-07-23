using Application.Features.Product.Dto;
using Application.Interfaces;

namespace Application.Features.Product.GetProduct
{
    public class GetProductQuery(int id) : ICacheableQuery<DetailedProductResponse>
    {
        public string CacheKey => throw new NotImplementedException();

        public TimeSpan? AbsoluteExpiration => throw new NotImplementedException();

        public TimeSpan? SlidingExpiration => throw new NotImplementedException();
    }
}
