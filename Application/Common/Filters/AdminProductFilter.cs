namespace Application.Common.Filters;

public class AdminProductFilter
{
    public string? Search { get; init; }

    public Guid? CategoryId { get; init; }

    public Guid? BrandId { get; init; }

    public bool? IsActive { get; init; }

    public bool? HasDiscount { get; init; }

    public bool? InStock { get; init; }

    public bool? IsLowStock { get; init; }

    public string? SortBy { get; init; }

    public bool SortDescending { get; init; }

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}
