using Domain.Entities.OrdersAggregate;
using Domain.Exceptions;

namespace Domain.Test.OrdersAggregate;

public class OrderItemTests
{
    private static readonly Guid ValidOrderId = Guid.NewGuid();
    private static readonly Guid ValidProductId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithValidArguments_ShouldInitializePropertiesAndCalculatedFields()
    {
        // Act
        var item = new OrderItem(
            orderId: ValidOrderId,
            productId: ValidProductId,
            quantity: 3,
            unitPrice: 20m,
            discountAmount: 2m);

        // Assert
        Assert.Equal(ValidOrderId, item.OrderId);
        Assert.Equal(ValidProductId, item.ProductId);
        Assert.Equal(3, item.Quantity);
        Assert.Equal(20m, item.UnitPrice);
        Assert.Equal(2m, item.DiscountAmount);
        Assert.Equal(60m, item.LineSubtotal);
        Assert.Equal(6m, item.LineDiscount);
        Assert.Equal(54m, item.LineTotal);
    }

    [Fact]
    public void Constructor_WithEmptyOrderId_ShouldThrowDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new OrderItem(Guid.Empty, ValidProductId, 1, 10m));
        Assert.Equal("Order id is required.", ex.Message);
    }

    [Fact]
    public void Constructor_WithEmptyProductId_ShouldThrowDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new OrderItem(ValidOrderId, Guid.Empty, 1, 10m));
        Assert.Equal("Product id is required.", ex.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithZeroOrNegativeQuantity_ShouldThrowDomainException(int quantity)
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new OrderItem(ValidOrderId, ValidProductId, quantity, 10m));
        Assert.Equal("Quantity must be greater than zero.", ex.Message);
    }

    [Fact]
    public void Constructor_WithNegativeUnitPrice_ShouldThrowDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new OrderItem(ValidOrderId, ValidProductId, 1, -10m));
        Assert.Equal("Unit price cannot be negative.", ex.Message);
    }

    [Fact]
    public void Constructor_WithNegativeDiscountAmount_ShouldThrowDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new OrderItem(ValidOrderId, ValidProductId, 1, 10m, -2m));
        Assert.Equal("Discount amount cannot be negative.", ex.Message);
    }

    [Fact]
    public void IncreaseQuantity_WithValidQuantity_ShouldUpdateQuantityAndCalculatedProperties()
    {
        // Arrange
        var item = new OrderItem(ValidOrderId, ValidProductId, quantity: 2, unitPrice: 50m, discountAmount: 5m);

        // Act
        item.IncreaseQuantity(3);

        // Assert
        Assert.Equal(5, item.Quantity);
        Assert.Equal(250m, item.LineSubtotal);
        Assert.Equal(25m, item.LineDiscount);
        Assert.Equal(225m, item.LineTotal);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void IncreaseQuantity_WithZeroOrNegative_ShouldThrowDomainException(int invalidQuantity)
    {
        // Arrange
        var item = new OrderItem(ValidOrderId, ValidProductId, 1, 10m);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => item.IncreaseQuantity(invalidQuantity));
        Assert.Equal("Quantity must be greater than zero.", ex.Message);
    }
}
