using System.ComponentModel.DataAnnotations;
using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities.ProcurementAggregate;

public class PurchaseOrder : AggregateRoot
{
    [MaxLength(50)]
    public string Number { get; private set; } = null!;

    public Guid SupplierId { get; private set; }

    public Supplier Supplier { get; private set; } = null!;

    public PurchaseOrderStatus Status { get; private set; }

    public DateOnly OrderDate { get; private set; }

    public DateOnly? ExpectedDeliveryDate { get; private set; }

    [Precision(18, 2)]
    public decimal Subtotal { get; private set; }

    [Precision(18, 2)]
    public decimal TaxAmount { get; private set; }

    [Precision(18, 2)]
    public decimal ShippingCost { get; private set; }

    [Precision(18, 2)]
    public decimal Total { get; private set; }

    [MaxLength(1000)]
    public string? Notes { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? ApprovedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public DateTime? CancelledAt { get; private set; }

    [Timestamp]
    public byte[] RowVersion { get; private set; } = [];

    public ICollection<PurchaseOrderItem> Items { get; private set; }
        = new List<PurchaseOrderItem>();

    public ICollection<GoodsReceipt> GoodsReceipts { get; private set; }
        = new List<GoodsReceipt>();

    private PurchaseOrder() { }

    public PurchaseOrder(
        string number,
        Guid supplierId,
        DateOnly orderDate,
        DateOnly? expectedDeliveryDate = null,
        string? notes = null)
        : base(Guid.NewGuid())
    {
        Number = ValidateRequiredText(
            number,
            50,
            "Purchase order number");

        if (supplierId == Guid.Empty)
            throw new DomainException("Supplier id is required.");

        if (expectedDeliveryDate.HasValue &&
            expectedDeliveryDate.Value < orderDate) {
            throw new DomainException(
                "Expected delivery date cannot be before order date.");
        }

        SupplierId = supplierId;
        OrderDate = orderDate;
        ExpectedDeliveryDate = expectedDeliveryDate;
        Notes = ValidateOptionalText(
            notes,
            1000,
            "Notes");

        Status = PurchaseOrderStatus.Draft;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddItem(
        Guid productId,
        int orderedQuantity,
        decimal unitCost)
    {
        EnsureDraft();

        if (productId == Guid.Empty)
            throw new DomainException("Product id is required.");

        if (orderedQuantity <= 0)
            throw new DomainException(
                "Ordered quantity must be greater than zero.");

        if (unitCost < 0)
            throw new DomainException(
                "Unit cost cannot be negative.");

        var existingItem =
            Items.FirstOrDefault(x => x.ProductId == productId);

        if (existingItem is not null) {
            existingItem.IncreaseOrderedQuantity(orderedQuantity);
            RecalculateTotals();
            return;
        }

        Items.Add(new PurchaseOrderItem(
            Id,
            productId,
            orderedQuantity,
            unitCost));

        RecalculateTotals();
    }

    public void SetCosts(
        decimal taxAmount,
        decimal shippingCost)
    {
        if (taxAmount < 0)
            throw new DomainException(
                "Tax amount cannot be negative.");

        if (shippingCost < 0)
            throw new DomainException(
                "Shipping cost cannot be negative.");

        TaxAmount = taxAmount;
        ShippingCost = shippingCost;

        RecalculateTotals();
    }

    public void SubmitForApproval()
    {
        EnsureDraft();

        if (!Items.Any()) {
            throw new DomainException(
                "Cannot submit purchase order without items.");
        }

        Status = PurchaseOrderStatus.PendingApproval;
    }

    public void Approve()
    {
        if (Status != PurchaseOrderStatus.PendingApproval) {
            throw new DomainException(
                "Only pending approval purchase orders can be approved.");
        }

        Status = PurchaseOrderStatus.Approved;
        ApprovedAt = DateTime.UtcNow;
    }

    public void MarkAsPartiallyReceived()
    {
        if (Status != PurchaseOrderStatus.Approved &&
            Status != PurchaseOrderStatus.PartiallyReceived) {
            throw new DomainException(
                "Only approved purchase orders can be marked as partially received.");
        }

        Status = PurchaseOrderStatus.PartiallyReceived;
    }

    public void Complete()
    {
        if (Status != PurchaseOrderStatus.Approved &&
            Status != PurchaseOrderStatus.PartiallyReceived) {
            throw new DomainException(
                "Only approved or partially received purchase orders can be completed.");
        }

        Status = PurchaseOrderStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == PurchaseOrderStatus.Completed) {
            throw new DomainException(
                "Completed purchase orders cannot be cancelled.");
        }

        if (Status == PurchaseOrderStatus.Cancelled) {
            throw new DomainException(
                "Purchase order is already cancelled.");
        }

        Status = PurchaseOrderStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
    }

    private void EnsureDraft()
    {
        if (Status != PurchaseOrderStatus.Draft) {
            throw new DomainException(
                "Items can only be added while purchase order is draft.");
        }
    }

    private void RecalculateTotals()
    {
        Subtotal = Items.Sum(
            x => x.OrderedQuantity * x.UnitCost);

        Total = Subtotal + TaxAmount + ShippingCost;
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
