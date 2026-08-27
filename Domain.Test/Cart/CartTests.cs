using System;
using Domain.Entities.Carts;
using Domain.Exceptions;
using Xunit;

namespace Domain.Test.Carts;

public class CartTests
{
    private static readonly Guid DefaultUserId = Guid.NewGuid();
    private static readonly Guid DefaultProductId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithValidUserId_ShouldInitializeEmptyCart()
    {
        var cart = new Cart(DefaultUserId);

        Assert.Equal(DefaultUserId, cart.UserId);
        Assert.Empty(cart.Items);
    }

    [Fact]
    public void Constructor_WithEmptyUserId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => new Cart(Guid.Empty));
    }

    [Fact]
    public void AddItem_NewProduct_ShouldAddItemToCollection()
    {
        var cart = new Cart(DefaultUserId);

        cart.AddItem(DefaultProductId, quantity: 2, unitPrice: 50m, discountAmount: 5m);

        var item = Assert.Single(cart.Items);
        Assert.Equal(DefaultProductId, item.ProductId);
        Assert.Equal(2, item.Quantity);
        Assert.Equal(50m, item.UnitPrice);
        Assert.Equal(5m, item.DiscountAmount);
    }

    [Fact]
    public void AddItem_ExistingProduct_ShouldIncreaseQuantity()
    {
        var cart = new Cart(DefaultUserId);
        cart.AddItem(DefaultProductId, quantity: 2, unitPrice: 50m);

        cart.AddItem(DefaultProductId, quantity: 3, unitPrice: 50m);

        var item = Assert.Single(cart.Items);
        Assert.Equal(5, item.Quantity);
    }

    [Fact]
    public void AddItem_WithEmptyProductId_ShouldThrowDomainException()
    {
        var cart = new Cart(DefaultUserId);

        Assert.Throws<DomainException>(() => cart.AddItem(Guid.Empty, 1, 50m));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddItem_WithInvalidQuantity_ShouldThrowDomainException(int quantity)
    {
        var cart = new Cart(DefaultUserId);

        Assert.Throws<DomainException>(() => cart.AddItem(DefaultProductId, quantity, 50m));
    }

    [Fact]
    public void AddItem_WithNegativePrice_ShouldThrowDomainException()
    {
        var cart = new Cart(DefaultUserId);

        Assert.Throws<DomainException>(() => cart.AddItem(DefaultProductId, 1, -10m));
    }

    [Fact]
    public void AddItem_WithDiscountExceedingUnitPrice_ShouldThrowDomainException()
    {
        var cart = new Cart(DefaultUserId);

        Assert.Throws<DomainException>(() => cart.AddItem(DefaultProductId, 1, 50m, 60m));
    }

    [Fact]
    public void UpdateItemQuantity_ExistingItem_ShouldUpdateQuantity()
    {
        var cart = new Cart(DefaultUserId);
        cart.AddItem(DefaultProductId, quantity: 2, unitPrice: 50m);

        cart.UpdateItemQuantity(DefaultProductId, quantity: 5);

        var item = Assert.Single(cart.Items);
        Assert.Equal(5, item.Quantity);
    }

    [Fact]
    public void UpdateItemQuantity_NonExistingItem_ShouldThrowDomainException()
    {
        var cart = new Cart(DefaultUserId);

        Assert.Throws<DomainException>(() => cart.UpdateItemQuantity(DefaultProductId, 5));
    }

    [Fact]
    public void RemoveItem_ExistingItem_ShouldRemoveFromCollection()
    {
        var cart = new Cart(DefaultUserId);
        cart.AddItem(DefaultProductId, quantity: 2, unitPrice: 50m);

        cart.RemoveItem(DefaultProductId);

        Assert.Empty(cart.Items);
    }

    [Fact]
    public void RemoveItem_NonExistingItem_ShouldNoOp()
    {
        var cart = new Cart(DefaultUserId);

        cart.RemoveItem(DefaultProductId);

        Assert.Empty(cart.Items);
    }

    [Fact]
    public void Clear_ShouldRemoveAllItems()
    {
        var cart = new Cart(DefaultUserId);
        cart.AddItem(Guid.NewGuid(), 1, 10m);
        cart.AddItem(Guid.NewGuid(), 2, 20m);

        cart.Clear();

        Assert.Empty(cart.Items);
    }
}
