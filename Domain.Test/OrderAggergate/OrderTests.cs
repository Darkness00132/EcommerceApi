using Domain.Entities.OrdersAggregate;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Test.OrdersAggregate;

public class OrderTests
{
    private static readonly Guid ValidUserId = Guid.NewGuid();
    private static readonly Address ValidAddress = new("123 Main St", "Cairo", "01000000000");

    [Fact]
    public void Constructor_WithValidArguments_ShouldInitializePendingOrder()
    {
        // Act
        var order = new Order(ValidUserId, ValidAddress);

        // Assert
        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Equal(ValidUserId, order.UserId);
        Assert.Equal(ValidAddress, order.ShippingAddress);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Null(order.PromoCodeId);
        Assert.Empty(order.Items);
        Assert.Equal(0, order.Subtotal);
        Assert.Equal(0, order.Total);
        Assert.False(order.IsCompleted);
        Assert.True(order.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Constructor_WithEmptyUserId_ShouldThrowDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new Order(Guid.Empty, ValidAddress));
        Assert.Equal("User id is required.", ex.Message);
    }

    [Fact]
    public void Constructor_WithNullShippingAddress_ShouldThrowDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new Order(ValidUserId, null!));
        Assert.Equal("Shipping address is required.", ex.Message);
    }

    [Fact]
    public void AddItem_WhenItemDoesNotExist_ShouldAddNewItemAndRecalculateTotals()
    {
        // Arrange
        var order = new Order(ValidUserId, ValidAddress);
        var productId = Guid.NewGuid();

        // Act
        order.AddItem(productId, quantity: 2, unitPrice: 50m, discountAmount: 5m);

        // Assert
        var item = Assert.Single(order.Items);
        Assert.Equal(order.Id, item.OrderId);
        Assert.Equal(productId, item.ProductId);
        Assert.Equal(2, item.Quantity);
        Assert.Equal(50m, item.UnitPrice);
        Assert.Equal(5m, item.DiscountAmount);

        Assert.Equal(100m, order.Subtotal);
        Assert.Equal(10m, order.ItemsDiscountAmount);
        Assert.Equal(90m, order.Total);
    }

    [Fact]
    public void AddItem_WhenItemAlreadyExists_ShouldIncreaseQuantityAndRecalculateTotals()
    {
        // Arrange
        var order = new Order(ValidUserId, ValidAddress);
        var productId = Guid.NewGuid();

        // Act
        order.AddItem(productId, quantity: 2, unitPrice: 50m, discountAmount: 5m);
        order.AddItem(productId, quantity: 3, unitPrice: 50m, discountAmount: 5m);

        // Assert
        var item = Assert.Single(order.Items);
        Assert.Equal(5, item.Quantity);

        Assert.Equal(250m, order.Subtotal);
        Assert.Equal(25m, order.ItemsDiscountAmount);
        Assert.Equal(225m, order.Total);
    }

    [Fact]
    public void AddItem_WhenOrderIsNotPending_ShouldThrowDomainException()
    {
        // Arrange
        var order = CreateConfirmedOrder();

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => order.AddItem(Guid.NewGuid(), 1, 10m));
        Assert.Equal("Cannot modify order items when order status is Confirmed.", ex.Message);
    }

    [Fact]
    public void SetShippingFee_WithValidAmount_ShouldUpdateShippingFeeAndRecalculateTotals()
    {
        // Arrange
        var order = new Order(ValidUserId, ValidAddress);
        order.AddItem(Guid.NewGuid(), 1, 100m);

        // Act
        order.SetShippingFee(15m);

        // Assert
        Assert.Equal(15m, order.ShippingFee);
        Assert.Equal(115m, order.Total);
    }

    [Fact]
    public void SetShippingFee_WithNegativeAmount_ShouldThrowDomainException()
    {
        // Arrange
        var order = new Order(ValidUserId, ValidAddress);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => order.SetShippingFee(-5m));
        Assert.Equal("Shipping fee cannot be negative.", ex.Message);
    }

    [Fact]
    public void ApplyPromoCode_WithValidArguments_ShouldUpdatePromoDiscountAndRecalculateTotals()
    {
        // Arrange
        var order = new Order(ValidUserId, ValidAddress);
        order.AddItem(Guid.NewGuid(), 1, 100m);
        var promoId = Guid.NewGuid();

        // Act
        order.ApplyPromoCode(promoId, 20m);

        // Assert
        Assert.Equal(promoId, order.PromoCodeId);
        Assert.Equal(20m, order.PromoDiscountAmount);
        Assert.Equal(80m, order.Total);
    }

    [Fact]
    public void ApplyPromoCode_WithEmptyPromoId_ShouldThrowDomainException()
    {
        // Arrange
        var order = new Order(ValidUserId, ValidAddress);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => order.ApplyPromoCode(Guid.Empty, 10m));
        Assert.Equal("Promo code id is required.", ex.Message);
    }

    [Fact]
    public void ApplyPromoCode_WithNegativeDiscount_ShouldThrowDomainException()
    {
        // Arrange
        var order = new Order(ValidUserId, ValidAddress);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => order.ApplyPromoCode(Guid.NewGuid(), -10m));
        Assert.Equal("Promo discount amount cannot be negative.", ex.Message);
    }

    [Fact]
    public void RemovePromoCode_ShouldClearPromoDataAndRecalculateTotals()
    {
        // Arrange
        var order = new Order(ValidUserId, ValidAddress);
        order.AddItem(Guid.NewGuid(), 1, 100m);
        order.ApplyPromoCode(Guid.NewGuid(), 20m);

        // Act
        order.RemovePromoCode();

        // Assert
        Assert.Null(order.PromoCodeId);
        Assert.Equal(0m, order.PromoDiscountAmount);
        Assert.Equal(100m, order.Total);
    }

    [Fact]
    public void RecalculateTotals_WhenDiscountsExceedSubtotal_ShouldClampTotalToZero()
    {
        // Arrange
        var order = new Order(ValidUserId, ValidAddress);
        order.AddItem(Guid.NewGuid(), 1, 50m);

        // Act
        order.ApplyPromoCode(Guid.NewGuid(), 100m);

        // Assert
        Assert.Equal(0m, order.Total);
    }

    [Fact]
    public void ChangeShippingAddress_WhenPending_ShouldUpdateAddress()
    {
        // Arrange
        var order = new Order(ValidUserId, ValidAddress);
        var newAddress = new Address("456 Elm St", "Giza", "01111111111");

        // Act
        order.ChangeShippingAddress(newAddress);

        // Assert
        Assert.Equal(newAddress, order.ShippingAddress);
    }

    [Fact]
    public void ChangeShippingAddress_WhenNotPending_ShouldThrowDomainException()
    {
        // Arrange
        var order = CreateConfirmedOrder();

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => order.ChangeShippingAddress(ValidAddress));
        Assert.Equal("Cannot change shipping address when order status is Confirmed.", ex.Message);
    }

    [Fact]
    public void Lifecycle_ShouldTransitionStatusThroughCompleteFlow()
    {
        // Arrange
        var order = new Order(ValidUserId, ValidAddress);
        order.AddItem(Guid.NewGuid(), 1, 100m);

        // Act & Assert: Pending -> Confirmed
        order.Confirm();
        Assert.Equal(OrderStatus.Confirmed, order.Status);
        Assert.False(order.IsCompleted);

        // Act & Assert: Confirmed -> Processing
        order.StartProcessing();
        Assert.Equal(OrderStatus.Processing, order.Status);
        Assert.False(order.IsCompleted);

        // Act & Assert: Processing -> Shipped
        order.Ship();
        Assert.Equal(OrderStatus.Shipped, order.Status);
        Assert.False(order.IsCompleted);

        // Act & Assert: Shipped -> Delivered
        order.Deliver();
        Assert.Equal(OrderStatus.Delivered, order.Status);
        Assert.True(order.IsCompleted);

        // Act & Assert: Delivered -> Refunded
        order.MarkAsRefunded();
        Assert.Equal(OrderStatus.Refunded, order.Status);
        Assert.True(order.IsCompleted);
    }

    [Fact]
    public void Confirm_WithoutItems_ShouldThrowDomainException()
    {
        // Arrange
        var order = new Order(ValidUserId, ValidAddress);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => order.Confirm());
        Assert.Equal("Cannot confirm an order without items.", ex.Message);
    }

    [Fact]
    public void Cancel_WhenPending_ShouldTransitionToCancelled()
    {
        // Arrange
        var order = new Order(ValidUserId, ValidAddress);

        // Act
        order.Cancel();

        // Assert
        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Cancel_WhenDelivered_ShouldThrowDomainException()
    {
        // Arrange
        var order = CreateDeliveredOrder();

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => order.Cancel());
        Assert.Equal("Delivered or refunded orders cannot be cancelled.", ex.Message);
    }

    private static Order CreateConfirmedOrder()
    {
        var order = new Order(ValidUserId, ValidAddress);
        order.AddItem(Guid.NewGuid(), 1, 100m);
        order.Confirm();
        return order;
    }

    private static Order CreateDeliveredOrder()
    {
        var order = CreateConfirmedOrder();
        order.StartProcessing();
        order.Ship();
        order.Deliver();
        return order;
    }
}
