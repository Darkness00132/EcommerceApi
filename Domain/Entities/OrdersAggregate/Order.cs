using Domain.Common;
using Domain.Entities.Identity;
using Domain.Entities.PromotionsAggregate;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.OrdersAggregate;

public sealed class Order : AggregateRoot
{
    public Guid UserId { get; private set; }

    public AppUser User { get; private set; } = null!;

    public OrderStatus Status { get; private set; }

    public decimal Subtotal { get; private set; }

    public decimal ShippingFee { get; private set; }

    public decimal ItemsDiscountAmount { get; private set; }

    public decimal PromoDiscountAmount { get; private set; }

    public decimal Total { get; private set; }

    public Address ShippingAddress { get; private set; } = null!;

    public Guid? PromoCodeId { get; private set; }
    public PromoCode? PromoCode { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();

    [NotMapped]
    public bool IsCompleted => Status is OrderStatus.Delivered or OrderStatus.Refunded;

    private Order() { }

    public Order(
        Guid userId,
        Address shippingAddress,
        Guid? promoCodeId = null)
        : base(Guid.NewGuid())
    {
        if (userId == Guid.Empty)
            throw new DomainException("User id is required.");

        UserId = userId;
        ShippingAddress = shippingAddress;
        PromoCodeId = promoCodeId;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddItem(
        Guid productId,
        int quantity,
        decimal unitPrice,
        decimal discountAmount = 0)
    {
        if (productId == Guid.Empty)
            throw new DomainException("Product id is required.");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        if (unitPrice < 0)
            throw new DomainException("Unit price cannot be negative.");

        if (discountAmount < 0)
            throw new DomainException("Discount amount cannot be negative.");

        var existingItem = Items.FirstOrDefault(x => x.ProductId == productId);

        if (existingItem is not null)
        {
            existingItem.IncreaseQuantity(quantity);
            RecalculateTotals();
            return;
        }

        Items.Add(new OrderItem(
            orderId: Id,
            productId: productId,
            quantity: quantity,
            unitPrice: unitPrice,
            discountAmount: discountAmount));

        RecalculateTotals();
    }

    public void SetShippingFee(decimal shippingFee)
    {
        if (shippingFee < 0)
            throw new DomainException("Shipping fee cannot be negative.");

        ShippingFee = shippingFee;

        RecalculateTotals();
    }

    public void ApplyPromoCode(Guid promoCodeId, decimal promoDiscountAmount)
    {
        if (promoCodeId == Guid.Empty)
            throw new DomainException("Promo code id is required.");

        if (promoDiscountAmount < 0)
            throw new DomainException("Promo discount amount cannot be negative.");

        PromoCodeId = promoCodeId;
        PromoDiscountAmount = promoDiscountAmount;

        RecalculateTotals();
    }

    public void RemovePromoCode()
    {
        PromoCodeId = null;
        PromoDiscountAmount = 0;

        RecalculateTotals();
    }

    public void ChangeShippingAddress(Address shippingAddress)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Shipping address can only be changed while order is pending.");

        ShippingAddress = shippingAddress;
    }

    public void Confirm()
    {
        if (!Items.Any())
            throw new DomainException("Cannot confirm an order without items.");

        if (Status != OrderStatus.Pending)
            throw new DomainException("Only pending orders can be confirmed.");

        Status = OrderStatus.Confirmed;
    }

    public void StartProcessing()
    {
        if (Status != OrderStatus.Confirmed)
            throw new DomainException("Only confirmed orders can be processed.");

        Status = OrderStatus.Processing;
    }

    public void Ship()
    {
        if (Status != OrderStatus.Processing)
            throw new DomainException("Only processing orders can be shipped.");

        Status = OrderStatus.Shipped;
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new DomainException("Only shipped orders can be delivered.");

        Status = OrderStatus.Delivered;
    }

    public void Cancel()
    {
        if (Status is OrderStatus.Delivered or OrderStatus.Refunded)
            throw new DomainException("Delivered or refunded orders cannot be cancelled.");

        Status = OrderStatus.Cancelled;
    }

    public void MarkAsRefunded()
    {
        if (Status != OrderStatus.Delivered)
            throw new DomainException("Only delivered orders can be refunded.");

        Status = OrderStatus.Refunded;
    }

    private void RecalculateTotals()
    {
        Subtotal = Items.Sum(x => x.UnitPrice * x.Quantity);
        ItemsDiscountAmount = Items.Sum(x => x.DiscountAmount * x.Quantity);

        Total = Subtotal + ShippingFee - ItemsDiscountAmount - PromoDiscountAmount;

        if (Total < 0)
            Total = 0;
    }
}