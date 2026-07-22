using Application.Features.Product.Dto;
using Application.Common.Pagination;
using Application.Interfaces;

namespace Application.Features.Category.GetProductsWithinCategory
{
    public record GetProductsWithinCategoryQuery(int id, PaginationRequest Pagination) : ICacheableQuery<PaginationResult<ProductItemResponse>>
    {
        public string CacheKey => $"categories:{id}:products:page:{Pagination.PageNumber}:size:{Pagination.PageSize}";

        public TimeSpan? AbsoluteExpiration => Pagination.PageNumber switch
        {
            1 => TimeSpan.FromHours(1),
            2 or 3 => TimeSpan.FromMinutes(30),
            _ => null
        };

        public TimeSpan? SlidingExpiration => Pagination.PageNumber switch
        {
            1 => TimeSpan.FromMinutes(10),
            2 or 3 => TimeSpan.FromMinutes(5),
            _ => null
        };
    }
}
