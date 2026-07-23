namespace Application.Common.Filters
{
    public record ProductFilters(string? SearchTerm,
        int? CategoryId,
        string? Brand,
        bool? InStock,
        bool? HasDiscount,
        decimal? MinPrice,
        decimal? MaxPrice,
        string? SortBy);
}
