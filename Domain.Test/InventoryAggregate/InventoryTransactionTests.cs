using Domain.Entities.InventoryAggregate;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Test.InventoryAggregate;

public class InventoryTransactionTests
{
    private static readonly Guid ValidInventoryId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithValidArguments_ShouldInitializePropertiesAndTrimNotes()
    {
        // Act
        var transaction = new InventoryTransaction(
            inventoryId: ValidInventoryId,
            type: InventoryTransactionType.StockIn,
            quantityChange: 10,
            quantityBefore: 20,
            quantityAfter: 30,
            goodsReceiptId: null,
            orderId: null,
            notes: "  Received new shipment  ");

        // Assert
        Assert.NotEqual(Guid.Empty, transaction.Id);
        Assert.Equal(ValidInventoryId, transaction.InventoryId);
        Assert.Equal(InventoryTransactionType.StockIn, transaction.Type);
        Assert.Equal(10, transaction.QuantityChange);
        Assert.Equal(20, transaction.QuantityBefore);
        Assert.Equal(30, transaction.QuantityAfter);
        Assert.Null(transaction.GoodsReceiptId);
        Assert.Null(transaction.OrderId);
        Assert.Equal("Received new shipment", transaction.Notes);
        Assert.True(transaction.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Constructor_WithEmptyInventoryId_ShouldThrowDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new InventoryTransaction(
            inventoryId: Guid.Empty,
            type: InventoryTransactionType.StockIn,
            quantityChange: 5,
            quantityBefore: 10,
            quantityAfter: 15));

        Assert.Equal("Inventory ID cannot be empty.", ex.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrWhitespaceNotes_ShouldSetNotesToNull(string? notes)
    {
        // Act
        var transaction = new InventoryTransaction(
            inventoryId: ValidInventoryId,
            type: InventoryTransactionType.StockOut,
            quantityChange: -5,
            quantityBefore: 10,
            quantityAfter: 5,
            notes: notes);

        // Assert
        Assert.Null(transaction.Notes);
    }
}
