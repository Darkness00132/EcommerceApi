using Application.Common.Filters;
using Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Extensions;

public static class ProductQueryExtensions
{
    public static IQueryable<Product> ApplyFilters(
        this IQueryable<Product> query,
        ProductFilter? filters)
    {
        if (filters is null)
            return query;

        if (!string.IsNullOrWhiteSpace(filters.Search)) {
            var search = filters.Search.Trim();

            query = query.Where(p =>
                EF.Functions.Like(p.NameEn, $"%{search}%") ||
                EF.Functions.Like(p.NameAr, $"%{search}%") ||
                EF.Functions.Like(p.DescriptionEn, $"%{search}%") ||
                EF.Functions.Like(p.DescriptionAr, $"%{search}%"));
        }

        if (filters.CategoryId is not null) {
            query = query.Where(p =>
                p.CategoryId == filters.CategoryId);
        }

        if (filters.BrandId is not null) {
            query = query.Where(p =>
                p.BrandId == filters.BrandId);
        }

        if (filters.MinPrice is not null) {
            query = query.Where(p =>
                p.Price >= filters.MinPrice);
        }

        if (filters.MaxPrice is not null) {
            query = query.Where(p =>
                p.Price <= filters.MaxPrice);
        }

        if (filters.HasDiscount == true) {
            query = query.Where(p => p.Discount != null);
        }

        if (filters.InStock == true) {
            query = query.Where(p => p.Inventory.QuantityOnHand > 0);
        }

        return query;
    }
}
