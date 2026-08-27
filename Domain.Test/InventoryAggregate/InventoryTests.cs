using Domain.Entities.InventoryAggregate;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Test.InventoryAggregate;

public class InventoryTests
{
    private static readonly Guid ValidProductId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithValidArguments_ShouldInitializePropertiesCorrectly()
    {
        // Act
        var inventory = new Inventory(ValidProductId, quantityOnHand: 50, reorderLevel: 10);

        // Assert
        Assert.NotEqual(Guid.Empty, inventory.Id);
        Assert.Equal(ValidProductId, inventory.ProductId);
        Assert.Equal(50, inventory.QuantityOnHand);
        Assert.Equal(10, inventory.ReorderLevel);
        Assert.Empty(inventory.Transactions);
    }

    [Fact]
    public void Constructor_WithEmptyProductId_ShouldThrowDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new Inventory(Guid.Empty, 10, 5));
        Assert.Equal("Product ID cannot be empty.", ex.Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_WithNegativeQuantity_ShouldThrowDomainException(int invalidQuantity)
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new Inventory(ValidProductId, invalidQuantity, 5));
        Assert.Equal("Quantity on hand cannot be negative.", ex.Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-5)]
    public void Constructor_WithNegativeReorderLevel_ShouldThrowDomainException(int invalidReorderLevel)
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new Inventory(ValidProductId, 10, invalidReorderLevel));
        Assert.Equal("Reorder level cannot be negative.", ex.Message);
    }

    [Fact]
    public void IncreaseStock_WithValidQuantity_ShouldUpdateStockAndRecordTransaction()
    {
        // Arrange
        var inventory = new Inventory(ValidProductId, quantityOnHand: 20, reorderLevel: 5);
        var receiptId = Guid.NewGuid();

        // Act
        inventory.IncreaseStock(10, goodsReceiptId: receiptId, notes: "  Shipment arrived  ");

        // Assert
        Assert.Equal(30, inventory.QuantityOnHand);

        var tx = Assert.Single(inventory.Transactions);
        Assert.Equal(inventory.Id, tx.InventoryId);
        Assert.Equal(InventoryTransactionType.StockIn, tx.Type);
        Assert.Equal(10, tx.QuantityChange);
        Assert.Equal(20, tx.QuantityBefore);
        Assert.Equal(30, tx.QuantityAfter);
        Assert.Equal(receiptId, tx.GoodsReceiptId);
        Assert.Null(tx.OrderId);
        Assert.Equal("Shipment arrived", tx.Notes);
        Assert.True(tx.CreatedAt <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void IncreaseStock_WithZeroOrNegativeQuantity_ShouldThrowDomainException(int invalidQuantity)
    {
        // Arrange
        var inventory = new Inventory(ValidProductId, quantityOnHand: 20, reorderLevel: 5);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => inventory.IncreaseStock(invalidQuantity));
        Assert.Equal("Quantity must be greater than zero.", ex.Message);
    }

    [Fact]
    public void DecreaseStock_WithValidQuantity_ShouldUpdateStockAndRecordTransaction()
    {
        // Arrange
        var inventory = new Inventory(ValidProductId, quantityOnHand: 20, reorderLevel: 5);
        var orderId = Guid.NewGuid();

        // Act
        inventory.DecreaseStock(8, orderId: orderId);

        // Assert
        Assert.Equal(12, inventory.QuantityOnHand);

        var tx = Assert.Single(inventory.Transactions);
        Assert.Equal(inventory.Id, tx.InventoryId);
        Assert.Equal(InventoryTransactionType.StockOut, tx.Type);
        Assert.Equal(-8, tx.QuantityChange);
        Assert.Equal(20, tx.QuantityBefore);
        Assert.Equal(12, tx.QuantityAfter);
        Assert.Equal(orderId, tx.OrderId);
        Assert.Null(tx.GoodsReceiptId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void DecreaseStock_WithZeroOrNegativeQuantity_ShouldThrowDomainException(int invalidQuantity)
    {
        // Arrange
        var inventory = new Inventory(ValidProductId, quantityOnHand: 20, reorderLevel: 5);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => inventory.DecreaseStock(invalidQuantity));
        Assert.Equal("Quantity must be greater than zero.", ex.Message);
    }

    [Fact]
    public void DecreaseStock_WhenQuantityExceedsStock_ShouldThrowDomainException()
    {
        // Arrange
        var inventory = new Inventory(ValidProductId, quantityOnHand: 5, reorderLevel: 2);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => inventory.DecreaseStock(10));
        Assert.Equal("Insufficient stock.", ex.Message);
    }

    [Fact]
    public void AdjustStock_ToHigherQuantity_ShouldUpdateStockAndRecordPositiveAdjustment()
    {
        // Arrange
        var inventory = new Inventory(ValidProductId, quantityOnHand: 10, reorderLevel: 2);

        // Act
        inventory.AdjustStock(15, notes: "Found extra items during audit");

        // Assert
        Assert.Equal(15, inventory.QuantityOnHand);

        var tx = Assert.Single(inventory.Transactions);
        Assert.Equal(InventoryTransactionType.Adjustment, tx.Type);
        Assert.Equal(5, tx.QuantityChange);
        Assert.Equal(10, tx.QuantityBefore);
        Assert.Equal(15, tx.QuantityAfter);
        Assert.Equal("Found extra items during audit", tx.Notes);
    }

    [Fact]
    public void AdjustStock_ToLowerQuantity_ShouldUpdateStockAndRecordNegativeAdjustment()
    {
        // Arrange
        var inventory = new Inventory(ValidProductId, quantityOnHand: 10, reorderLevel: 2);

        // Act
        inventory.AdjustStock(6);

        // Assert
        Assert.Equal(6, inventory.QuantityOnHand);

        var tx = Assert.Single(inventory.Transactions);
        Assert.Equal(InventoryTransactionType.Adjustment, tx.Type);
        Assert.Equal(-4, tx.QuantityChange);
        Assert.Equal(10, tx.QuantityBefore);
        Assert.Equal(6, tx.QuantityAfter);
    }

    [Fact]
    public void AdjustStock_WithNegativeValue_ShouldThrowDomainException()
    {
        // Arrange
        var inventory = new Inventory(ValidProductId, quantityOnHand: 10, reorderLevel: 2);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => inventory.AdjustStock(-1));
        Assert.Equal("New quantity cannot be negative.", ex.Message);
    }

    [Fact]
    public void ChangeReorderLevel_WithValidValue_ShouldUpdateReorderLevel()
    {
        // Arrange
        var inventory = new Inventory(ValidProductId, quantityOnHand: 10, reorderLevel: 5);

        // Act
        inventory.ChangeReorderLevel(15);

        // Assert
        Assert.Equal(15, inventory.ReorderLevel);
    }

    [Fact]
    public void ChangeReorderLevel_WithNegativeValue_ShouldThrowDomainException()
    {
        // Arrange
        var inventory = new Inventory(ValidProductId, quantityOnHand: 10, reorderLevel: 5);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => inventory.ChangeReorderLevel(-1));
        Assert.Equal("Reorder level cannot be negative.", ex.Message);
    }
}
