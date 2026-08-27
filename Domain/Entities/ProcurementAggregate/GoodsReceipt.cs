using System.ComponentModel.DataAnnotations;
using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities.ProcurementAggregate;

public sealed class GoodsReceipt : AggregateRoot
{
    [MaxLength(50)]
    public string Number { get; private set; } = null!;

    public Guid PurchaseOrderId { get; private set; }

    public PurchaseOrder PurchaseOrder { get; private set; } = null!;

    public GoodsReceiptStatus Status { get; private set; }

    public DateTime ReceivedAt { get; private set; }

    [MaxLength(100)]
    public string? DeliveryReference { get; private set; }

    [MaxLength(1000)]
    public string? Notes { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? ConfirmedAt { get; private set; }

    public DateTime? CancelledAt { get; private set; }

    public ICollection<GoodsReceiptItem> Items { get; private set; }
        = new List<GoodsReceiptItem>();

    private GoodsReceipt() { }

    public GoodsReceipt(
        string number,
        Guid purchaseOrderId,
        DateTime receivedAt,
        string? deliveryReference = null,
        string? notes = null)
        : base(Guid.NewGuid())
    {
        Number = ValidateRequiredText(number, 50, "Goods receipt number");

        if (purchaseOrderId == Guid.Empty)
            throw new DomainException("Purchase order id is required.");

        PurchaseOrderId = purchaseOrderId;
        ReceivedAt = receivedAt;
        DeliveryReference = ValidateOptionalText(
            deliveryReference,
            100,
            "Delivery reference");

        Notes = ValidateOptionalText(notes, 1000, "Notes");
        Status = GoodsReceiptStatus.Draft;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddItem(Guid productId, int quantity)
    {
        EnsureDraft();

        if (productId == Guid.Empty)
            throw new DomainException("Product id is required.");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        var existingItem = Items.FirstOrDefault(x => x.ProductId == productId);

        if (existingItem is not null) {
            existingItem.IncreaseQuantity(quantity);
            return;
        }

        Items.Add(new GoodsReceiptItem(
            Id,
            productId,
            quantity));
    }

    public void Confirm()
    {
        EnsureDraft();

        if (Items.Count == 0)
            throw new DomainException(
                "Cannot confirm goods receipt without items.");

        Status = GoodsReceiptStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        EnsureDraft();

        Status = GoodsReceiptStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
    }

    private void EnsureDraft()
    {
        if (Status != GoodsReceiptStatus.Draft)
            throw new DomainException(
                "Only draft goods receipts can be modified.");
    }

    private static string ValidateRequiredText(
        string value,
        int maxLength,
        string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException($"{fieldName} is required.");

        var trimmedValue = value.Trim();

        if (trimmedValue.Length > maxLength) {
            throw new DomainException(
                $"{fieldName} cannot exceed {maxLength} characters.");
        }

        return trimmedValue;
    }

    private static string? ValidateOptionalText(
        string? value,
        int maxLength,
        string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var trimmedValue = value.Trim();

        if (trimmedValue.Length > maxLength) {
            throw new DomainException(
                $"{fieldName} cannot exceed {maxLength} characters.");
        }

        return trimmedValue;
    }
}
