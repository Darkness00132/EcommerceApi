using Api.Contracts.Common;
using Application.Common.Filters;
using Application.Common.Pagination;
using Application.Constants;
using Application.Features.Products.Dtos;

namespace Application.Features.Products.Queries.GetProducts;

public record GetProductsQuery(PaginationRequest Pagination, ProductFilter? Filters)
    : ICacheableQuery<PagedResult<ProductInList>>
{
    public string CacheKey
        => $"{CacheNames.Products}:{Pagination.PageNumber}:{Pagination.PageSize}";

    public IReadOnlyCollection<string> Tags => [CacheNames.Products];

    public CacheOptions CacheOptions => new CacheOptions() {
        AbsoluteExpiration = Pagination.PageNumber switch {
            1 => TimeSpan.FromDays(7),
            2 or 3 => TimeSpan.FromDays(2),
            > 10 => TimeSpan.FromHours(6),
            _ => TimeSpan.FromMinutes(30)
        }
    };

    public bool BypassCache => Filters is not null;
}
