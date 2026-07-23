using Application.Common.Filters;
using Application.Common.Pagination;
using Application.Features.Product.Dto;
using Application.Interfaces;

namespace Application.Features.Product.GetProducts
{
    public record GetProductsQuery(ProductFilters filters,PaginationRequest Pagination) : ICacheableQuery<PaginationResult<ProductItemResponse>>
    {
        public string CacheKey => throw new NotImplementedException();

        public TimeSpan? AbsoluteExpiration => throw new NotImplementedException();

        public TimeSpan? SlidingExpiration => throw new NotImplementedException();
    }
}
