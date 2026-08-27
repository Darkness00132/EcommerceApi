using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Entities.Catalog;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities.Carts;

public sealed class CartItem : IEntity
{
    public Guid CartId { get; private set; }

    public Guid ProductId { get; private set; }

    public Cart Cart { get; private set; } = null!;

    public Product Product { get; private set; } = null!;

    public int Quantity { get; private set; }

    [Precision(18,2)]
    public decimal UnitPrice { get; private set; }

    [Precision(18, 2)]
    public decimal DiscountAmount { get; private set; }

    [NotMapped]
    public decimal LineSubtotal => UnitPrice * Quantity;

    [NotMapped]
    public decimal LineDiscount => DiscountAmount * Quantity;

    [NotMapped]
    public decimal LineTotal => LineSubtotal - LineDiscount;

    private CartItem() { }

    public CartItem(
        Guid cartId,
        Guid productId,
        int quantity,
        decimal unitPrice,
        decimal discountAmount = 0)
    {
        if (cartId == Guid.Empty)
            throw new DomainException("Cart ID cannot be empty.");

        if (productId == Guid.Empty)
            throw new DomainException("Product ID cannot be empty.");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        if (unitPrice < 0)
            throw new DomainException("Unit price cannot be negative.");

        if (discountAmount < 0)
            throw new DomainException("Discount amount cannot be negative.");

        if (discountAmount > unitPrice)
            throw new DomainException("Discount amount cannot exceed unit price.");

        CartId = cartId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        DiscountAmount = discountAmount;
    }

    public void IncreaseQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        Quantity += quantity;
    }

    public void ChangeQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        Quantity = quantity;
    }

    public void ChangePrice(decimal unitPrice, decimal discountAmount = 0)
    {
        if (unitPrice < 0)
            throw new DomainException("Unit price cannot be negative.");

        if (discountAmount < 0)
            throw new DomainException("Discount amount cannot be negative.");

        if (discountAmount > unitPrice)
            throw new DomainException("Discount amount cannot exceed unit price.");

        UnitPrice = unitPrice;
        DiscountAmount = discountAmount;
    }
}
