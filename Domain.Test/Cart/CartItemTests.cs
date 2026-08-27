using System;
using Domain.Entities.Carts;
using Domain.Exceptions;
using Xunit;

namespace Domain.Test.Carts;

public class CartItemTests
{
    private static readonly Guid DefaultCartId = Guid.NewGuid();
    private static readonly Guid DefaultProductId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithValidData_ShouldInitializePropertiesAndCalculations()
    {
        var item = new CartItem(DefaultCartId, DefaultProductId, quantity: 3, unitPrice: 100m, discountAmount: 10m);

        Assert.Equal(DefaultCartId, item.CartId);
        Assert.Equal(DefaultProductId, item.ProductId);
        Assert.Equal(3, item.Quantity);
        Assert.Equal(100m, item.UnitPrice);
        Assert.Equal(10m, item.DiscountAmount);
        Assert.Equal(300m, item.LineSubtotal);
        Assert.Equal(30m, item.LineDiscount);
        Assert.Equal(270m, item.LineTotal);
    }

    [Fact]
    public void Constructor_WithEmptyCartId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new CartItem(Guid.Empty, DefaultProductId, 1, 100m));
    }

    [Fact]
    public void Constructor_WithEmptyProductId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new CartItem(DefaultCartId, Guid.Empty, 1, 100m));
    }

    [Fact]
    public void IncreaseQuantity_ValidQuantity_ShouldIncreaseTotalQuantity()
    {
        var item = new CartItem(DefaultCartId, DefaultProductId, 2, 50m);

        item.IncreaseQuantity(3);

        Assert.Equal(5, item.Quantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void IncreaseQuantity_InvalidQuantity_ShouldThrowDomainException(int quantity)
    {
        var item = new CartItem(DefaultCartId, DefaultProductId, 2, 50m);

        Assert.Throws<DomainException>(() => item.IncreaseQuantity(quantity));
    }

    [Fact]
    public void ChangeQuantity_ValidQuantity_ShouldSetQuantity()
    {
        var item = new CartItem(DefaultCartId, DefaultProductId, 2, 50m);

        item.ChangeQuantity(10);

        Assert.Equal(10, item.Quantity);
    }

    [Fact]
    public void ChangePrice_ValidPriceAndDiscount_ShouldUpdateValues()
    {
        var item = new CartItem(DefaultCartId, DefaultProductId, 2, 50m, 5m);

        item.ChangePrice(80m, 10m);

        Assert.Equal(80m, item.UnitPrice);
        Assert.Equal(10m, item.DiscountAmount);
    }

    [Fact]
    public void ChangePrice_DiscountExceedsPrice_ShouldThrowDomainException()
    {
        var item = new CartItem(DefaultCartId, DefaultProductId, 2, 50m);

        Assert.Throws<DomainException>(() => item.ChangePrice(50m, 60m));
    }
}
