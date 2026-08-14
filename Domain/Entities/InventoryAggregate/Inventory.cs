using Domain.Common;
using Domain.Entities.Catalog;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities.InventoryAggregate;

public sealed class Inventory : AggregateRoot
{
    public Guid ProductId { get; private set; }

    public Product Product { get; private set; } = null!;

    public int QuantityOnHand { get; private set; }

    public int ReorderLevel { get; private set; }

    public byte[] RowVersion { get; private set; } = [];

    public ICollection<InventoryTransaction> Transactions { get; private set; } = new List<InventoryTransaction>();

    private Inventory() { }

    public Inventory(Guid productId, int quantityOnHand, int reorderLevel)
    {
        if (quantityOnHand < 0)
            throw new DomainException("Quantity on hand cannot be negative.");

        if (reorderLevel < 0)
            throw new DomainException("Reorder level cannot be negative.");

        Id = Guid.NewGuid();
        ProductId = productId;
        QuantityOnHand = quantityOnHand;
        ReorderLevel = reorderLevel;
    }

    public void IncreaseStock(
        int quantity,
        Guid? goodsReceiptId = null,
        string? notes = null)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        var quantityBefore = QuantityOnHand;
        QuantityOnHand += quantity;

        AddTransaction(
            type: InventoryTransactionType.StockIn,
            quantityChange: quantity,
            quantityBefore: quantityBefore,
            quantityAfter: QuantityOnHand,
            orderId: null,
            goodsReceiptId: goodsReceiptId,
            notes: notes);
    }

    public void DecreaseStock(
        int quantity,
        Guid? orderId = null,
        string? notes = null)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        if (QuantityOnHand < quantity)
            throw new DomainException("Insufficient stock.");

        var quantityBefore = QuantityOnHand;
        QuantityOnHand -= quantity;

        AddTransaction(
            type: InventoryTransactionType.StockOut,
            quantityChange: -quantity,
            quantityBefore: quantityBefore,
            quantityAfter: QuantityOnHand,
            orderId: orderId,
            goodsReceiptId: null,
            notes: notes);
    }

    public void AdjustStock(int newQuantity, string? notes = null)
    {
        if (newQuantity < 0)
            throw new DomainException("New quantity cannot be negative.");

        var quantityBefore = QuantityOnHand;
        var quantityChange = newQuantity - QuantityOnHand;

        QuantityOnHand = newQuantity;

        AddTransaction(
            type: InventoryTransactionType.Adjustment,
            quantityChange: quantityChange,
            quantityBefore: quantityBefore,
            quantityAfter: QuantityOnHand,
            orderId: null,
            goodsReceiptId: null,
            notes: notes);
    }

    public void ChangeReorderLevel(int reorderLevel)
    {
        if (reorderLevel < 0)
            throw new DomainException("Reorder level cannot be negative.");

        ReorderLevel = reorderLevel;
    }

    private void AddTransaction(
        InventoryTransactionType type,
        int quantityChange,
        int quantityBefore,
        int quantityAfter,
        Guid? orderId,
        Guid? goodsReceiptId,
        string? notes)
    {
        Transactions.Add(new InventoryTransaction(
            inventoryId: Id,
            type: type,
            quantityChange: quantityChange,
            quantityBefore: quantityBefore,
            quantityAfter: quantityAfter,
            orderId: orderId,
            goodsReceiptId: goodsReceiptId,
            notes: notes));
    }
}