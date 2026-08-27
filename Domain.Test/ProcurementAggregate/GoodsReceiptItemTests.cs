using Domain.Entities.ProcurementAggregate;
using Domain.Exceptions;

namespace Domain.Tests.ProcurementAggregate;

public class GoodsReceiptItemTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateItem()
    {
        var receiptId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var item = new GoodsReceiptItem(
            receiptId,
            productId,
            5);

        Assert.Equal(receiptId, item.GoodsReceiptId);
        Assert.Equal(productId, item.ProductId);
        Assert.Equal(5, item.Quantity);
    }

    [Fact]
    public void Constructor_WithEmptyGoodsReceiptId_ShouldThrow()
    {
        var action = () => new GoodsReceiptItem(
            Guid.Empty,
            Guid.NewGuid(),
            1);

        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Constructor_WithEmptyProductId_ShouldThrow()
    {
        var action = () => new GoodsReceiptItem(
            Guid.NewGuid(),
            Guid.Empty,
            1);

        Assert.Throws<DomainException>(action);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void Constructor_WithInvalidQuantity_ShouldThrow(
        int quantity)
    {
        var action = () => new GoodsReceiptItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            quantity);

        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void IncreaseQuantity_WithValidQuantity_ShouldIncreaseQuantity()
    {
        var item = new GoodsReceiptItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            5);

        item.IncreaseQuantity(3);

        Assert.Equal(8, item.Quantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void IncreaseQuantity_WithInvalidQuantity_ShouldThrow(
        int quantity)
    {
        var item = new GoodsReceiptItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            5);

        var action = () => item.IncreaseQuantity(quantity);

        Assert.Throws<DomainException>(action);
    }
}
