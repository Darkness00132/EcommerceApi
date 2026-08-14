using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities.InventoryAggregate;

public sealed class InventoryTransaction : Entity
{
    public Guid InventoryId { get; private set; }

    public Inventory Inventory { get; private set; } = null!;

    public InventoryTransactionType Type { get; private set; }

    public int QuantityChange { get; private set; }

    public int QuantityBefore { get; private set; }

    public int QuantityAfter { get; private set; }

    public Guid? OrderId { get; private set; }

    public Guid? GoodsReceiptId { get; private set; }

    public string? Notes { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private InventoryTransaction()
    {
    }
    public InventoryTransaction(
        Guid inventoryId,
        InventoryTransactionType type,
        int quantityChange,
        int quantityBefore,
        int quantityAfter,
        Guid? orderId = null,
        Guid? goodsReceiptId = null,
        string? notes = null)
        : base(Guid.NewGuid())
    {
        if (quantityBefore < 0)
            throw new DomainException("Quantity before cannot be negative.");

        if (quantityAfter < 0)
            throw new DomainException("Quantity after cannot be negative.");

        InventoryId = inventoryId;
        Type = type;
        QuantityChange = quantityChange;
        QuantityBefore = quantityBefore;
        QuantityAfter = quantityAfter;
        OrderId = orderId;
        GoodsReceiptId = goodsReceiptId;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        CreatedAt = DateTime.UtcNow;
    }
}