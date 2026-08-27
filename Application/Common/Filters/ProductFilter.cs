namespace Application.Common.Filters;

public class ProductFilter
{
    public string? Search { get; init; }
    public Guid? CategoryId { get; init; }
    public Guid? BrandId { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public bool? HasDiscount { get; init; }
    public bool? InStock { get; init; }
    public ProductSortBy SortBy { get; init; } = ProductSortBy.CreatedAt;
    public bool SortDescending { get; init; } = true;
}
public enum ProductSortBy
{
    CreatedAt,
    Price
}
