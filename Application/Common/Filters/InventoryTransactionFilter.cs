using Domain.Enums;

namespace Application.Common.Filters;

public class InventoryTransactionFilter
{
    public Guid? ProductId { get; init; }

    public InventoryTransactionType? Type { get; init; }

    public Guid? OrderId { get; init; }

    public Guid? GoodsReceiptId { get; init; }

    public DateTime? CreatedFrom { get; init; }

    public DateTime? CreatedTo { get; init; }

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}
