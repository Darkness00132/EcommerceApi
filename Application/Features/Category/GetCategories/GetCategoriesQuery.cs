using Application.Common.Pagination;
using Application.Features.Category.Dtos;
using Application.Interfaces;

namespace Application.Features.Category.GetCategories
{
    public record GetCategoriesQuery(PaginationRequest Pagination) : ICacheableQuery<PaginationResult<CategoryDto>>
    {
        public string CacheKey => Pagination.PageNumber > 3
            ? string.Empty
            : $"categories:page:{Pagination.PageNumber}:size:{Pagination.PageSize}";
        public TimeSpan? AbsoluteExpiration => Pagination.PageNumber switch
        {
            1 => TimeSpan.FromHours(2),
            2 or 3 => TimeSpan.FromMinutes(30),
            _ => null
        };
        public TimeSpan? SlidingExpiration => null;
    };
}
