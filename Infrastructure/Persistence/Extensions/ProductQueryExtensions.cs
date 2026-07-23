using Domain.Entities;

namespace Infrastructure.Persistence.Extensions
{
    internal static class ProductFilterExtensions
    {
        public static IQueryable<Product> ApplySearch(this IQueryable<Product> query, string? term)
        {
            if (string.IsNullOrWhiteSpace(term)) return query;
            term = term.Trim();
            return query.Where(p => p.NameEn.Contains(term) || p.NameAr.Contains(term) || p.SKU.Contains(term));
        }

        public static IQueryable<Product> ApplyCategory(this IQueryable<Product> query, int? categoryId)
            => categoryId.HasValue ? query.Where(p => p.CategoryId == categoryId.Value) : query;

        public static IQueryable<Product> ApplyBrand(this IQueryable<Product> query, string? brand)
            => !string.IsNullOrWhiteSpace(brand) ? query.Where(p => p.Brand == brand) : query;

        public static IQueryable<Product> ApplyStock(this IQueryable<Product> query, bool? inStock)
            => inStock == true ? query.Where(p => p.Stock > 0) : query;

        public static IQueryable<Product> ApplyPriceRange(this IQueryable<Product> query, decimal? min, decimal? max)
        {
            if (min.HasValue) query = query.Where(p => p.Price >= min.Value);
            if (max.HasValue) query = query.Where(p => p.Price <= max.Value);
            return query;
        }

        public static IQueryable<Product> ApplySorting(this IQueryable<Product> query, string? sortBy)
        {
            return sortBy?.ToLower() switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "newest" => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.Id)
            };
        }
    }
}
