using Domain.Entities.ProcurementAggregate;
using Domain.Enums;
using Domain.Exceptions;
using FluentAssertions;

namespace Domain.Tests.ProcurementAggregate;

public class GoodsReceiptTests
{
    [Fact]
    public void Goods_Receipt_Is_Draft_When_Created()
    {
        // Arrange & Act
        var receipt = CreateValidReceipt();

        // Assert
        receipt.Status.Should().Be(GoodsReceiptStatus.Draft);
        receipt.Items.Should().BeEmpty();
    }

    [Fact]
    public void Goods_Receipt_Contains_Received_Product_When_Product_Is_Added()
    {
        // Arrange
        var receipt = CreateValidReceipt();
        var productId = Guid.NewGuid();

        // Act
        receipt.AddItem(productId, 10);

        // Assert
        receipt.Items.Should().ContainSingle();
        receipt.Items.Single().ProductId.Should().Be(productId);
        receipt.Items.Single().Quantity.Should().Be(10);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public void Goods_Receipt_Records_The_Received_Quantity(
        int quantity)
    {
        // Arrange
        var receipt = CreateValidReceipt();

        // Act
        receipt.AddItem(Guid.NewGuid(), quantity);

        // Assert
        receipt.Items.Single().Quantity.Should().Be(quantity);
    }

    [Fact]
    public void Goods_Receipt_Combines_Quantities_When_The_Same_Product_Is_Received_Again()
    {
        // Arrange
        var receipt = CreateValidReceipt();
        var productId = Guid.NewGuid();

        receipt.AddItem(productId, 10);

        // Act
        receipt.AddItem(productId, 5);

        // Assert
        receipt.Items.Should().ContainSingle();
        receipt.Items.Single().Quantity.Should().Be(15);
    }

    [Fact]
    public void Goods_Receipt_Can_Be_Confirmed_When_It_Contains_Products()
    {
        // Arrange
        var receipt = CreateValidReceipt();
        receipt.AddItem(Guid.NewGuid(), 10);

        // Act
        receipt.Confirm();

        // Assert
        receipt.Status.Should().Be(GoodsReceiptStatus.Confirmed);
        receipt.ConfirmedAt.Should().NotBeNull();
    }

    [Fact]
    public void Goods_Receipt_Cannot_Be_Confirmed_Without_Products()
    {
        // Arrange
        var receipt = CreateValidReceipt();

        // Act
        var act = () => receipt.Confirm();

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Goods_Receipt_Can_Be_Cancelled_While_Draft()
    {
        // Arrange
        var receipt = CreateValidReceipt();

        // Act
        receipt.Cancel();

        // Assert
        receipt.Status.Should().Be(GoodsReceiptStatus.Cancelled);
        receipt.CancelledAt.Should().NotBeNull();
    }

    [Fact]
    public void Confirmed_Goods_Receipt_Cannot_Be_Changed()
    {
        // Arrange
        var receipt = CreateValidReceipt();
        receipt.AddItem(Guid.NewGuid(), 10);
        receipt.Confirm();

        // Act
        var act = () => receipt.AddItem(Guid.NewGuid(), 5);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Confirmed_Goods_Receipt_Cannot_Be_Cancelled()
    {
        // Arrange
        var receipt = CreateValidReceipt();
        receipt.AddItem(Guid.NewGuid(), 10);
        receipt.Confirm();

        // Act
        var act = () => receipt.Cancel();

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Cancelled_Goods_Receipt_Cannot_Be_Changed()
    {
        // Arrange
        var receipt = CreateValidReceipt();
        receipt.Cancel();

        // Act
        var act = () => receipt.AddItem(Guid.NewGuid(), 5);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Cancelled_Goods_Receipt_Cannot_Be_Confirmed()
    {
        // Arrange
        var receipt = CreateValidReceipt();
        receipt.Cancel();

        // Act
        var act = () => receipt.Confirm();

        // Assert
        act.Should().Throw<DomainException>();
    }

    private static GoodsReceipt CreateValidReceipt()
    {
        return new GoodsReceipt(
            "GR-001",
            Guid.NewGuid(),
            DateTime.UtcNow);
    }
}
