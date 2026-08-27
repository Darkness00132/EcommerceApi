namespace Application.Common.Filters;

public class InventoryFilter
{
    public string? Search { get; init; }

    public Guid? CategoryId { get; init; }

    public Guid? BrandId { get; init; }

    public bool? OutOfStock { get; init; }

    public bool? BelowReorderLevel { get; init; }

    public string? SortBy { get; init; }

    public bool SortDescending { get; init; }

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}
