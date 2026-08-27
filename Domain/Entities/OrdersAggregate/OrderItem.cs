using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Entities.Catalog;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities.OrdersAggregate;

public sealed class OrderItem : IEntity
{
    public Guid OrderId { get; private set; }

    public Guid ProductId { get; private set; }

    public Order Order { get; private set; } = null!;

    public Product Product { get; private set; } = null!;

    public int Quantity { get; private set; }

    [Precision(18, 2)]
    public decimal UnitPrice { get; private set; }

    [Precision(18, 2)]
    public decimal DiscountAmount { get; private set; }

    [NotMapped]
    public decimal LineSubtotal => UnitPrice * Quantity;

    [NotMapped]
    public decimal LineDiscount => DiscountAmount * Quantity;

    [NotMapped]
    public decimal LineTotal => LineSubtotal - LineDiscount;

    private OrderItem() { }

    internal OrderItem(
        Guid orderId,
        Guid productId,
        int quantity,
        decimal unitPrice,
        decimal discountAmount = 0)
    {
        if (orderId == Guid.Empty)
            throw new DomainException("Order id is required.");

        if (productId == Guid.Empty)
            throw new DomainException("Product id is required.");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        if (unitPrice < 0)
            throw new DomainException("Unit price cannot be negative.");

        if (discountAmount < 0)
            throw new DomainException("Discount amount cannot be negative.");

        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        DiscountAmount = discountAmount;
    }

    internal void IncreaseQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        Quantity += quantity;
    }
}
