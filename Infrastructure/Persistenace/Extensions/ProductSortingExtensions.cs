using Application.Common.Filters;
using Domain.Entities.Catalog;

namespace Infrastructure.Persistence.Extensions;

public static class ProductSortingExtensions
{
    public static IQueryable<Product> ApplySorting(
        this IQueryable<Product> query,
        ProductFilter? filters)
    {
        var descending = filters?.SortDescending ?? true;

        return filters?.SortBy switch {
            ProductSortBy.Price => descending
                ? query.OrderByDescending(p => p.Price)
                : query.OrderBy(p => p.Price),

            _ => descending
                ? query.OrderByDescending(p => p.CreatedAt)
                : query.OrderBy(p => p.CreatedAt)
        };
    }
}
