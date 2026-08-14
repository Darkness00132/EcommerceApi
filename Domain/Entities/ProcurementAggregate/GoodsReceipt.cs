using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities.ProcurementAggregate;

public sealed class GoodsReceipt : AggregateRoot
{
    public string Number { get; private set; } = null!;

    public Guid PurchaseOrderId { get; private set; }

    public PurchaseOrder PurchaseOrder { get; private set; } = null!;

    public GoodsReceiptStatus Status { get; private set; }

    public DateTime ReceivedAt { get; private set; }

    public string? DeliveryReference { get; private set; }

    public string? Notes { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? ConfirmedAt { get; private set; }

    public DateTime? CancelledAt { get; private set; }

    public ICollection<GoodsReceiptItem> Items { get; private set; } = new List<GoodsReceiptItem>();

    private GoodsReceipt() { }

    public GoodsReceipt(
        string number,
        Guid purchaseOrderId,
        DateTime receivedAt,
        string? deliveryReference = null,
        string? notes = null)
        : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new DomainException("Goods receipt number is required.");

        if (purchaseOrderId == Guid.Empty)
            throw new DomainException("Purchase order id is required.");

        Number = number.Trim();
        PurchaseOrderId = purchaseOrderId;
        ReceivedAt = receivedAt;
        DeliveryReference = string.IsNullOrWhiteSpace(deliveryReference) ? null : deliveryReference.Trim();
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        Status = GoodsReceiptStatus.Draft;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddItem(Guid productId, int quantity)
    {
        if (Status != GoodsReceiptStatus.Draft)
            throw new DomainException("Items can only be added while goods receipt is draft.");

        if (productId == Guid.Empty)
            throw new DomainException("Product id is required.");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        var existingItem = Items.FirstOrDefault(x => x.ProductId == productId);

        if (existingItem is not null)
        {
            existingItem.IncreaseQuantity(quantity);
            return;
        }

        Items.Add(new GoodsReceiptItem(
            goodsReceiptId: Id,
            productId: productId,
            quantity: quantity));
    }

    public void Confirm()
    {
        if (Status != GoodsReceiptStatus.Draft)
            throw new DomainException("Only draft goods receipts can be confirmed.");

        if (!Items.Any())
            throw new DomainException("Cannot confirm goods receipt without items.");

        Status = GoodsReceiptStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == GoodsReceiptStatus.Confirmed)
            throw new DomainException("Confirmed goods receipts cannot be cancelled.");

        Status = GoodsReceiptStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
    }
}