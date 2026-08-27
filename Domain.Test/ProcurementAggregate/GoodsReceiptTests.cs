using Domain.Entities.ProcurementAggregate;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Tests.ProcurementAggregate;

public sealed class GoodsReceiptTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateReceipt()
    {
        var purchaseOrderId = Guid.NewGuid();

        var receipt = new GoodsReceipt(
            "GR-001",
            purchaseOrderId,
            DateTime.UtcNow);

        Assert.Equal("GR-001", receipt.Number);
        Assert.Equal(purchaseOrderId, receipt.PurchaseOrderId);
        Assert.Equal(GoodsReceiptStatus.Draft, receipt.Status);
        Assert.Empty(receipt.Items);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WithInvalidNumber_ShouldThrow(
        string? number)
    {
        var action = () => new GoodsReceipt(
            number!,
            Guid.NewGuid(),
            DateTime.UtcNow);

        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Constructor_WithEmptyPurchaseOrderId_ShouldThrow()
    {
        var action = () => new GoodsReceipt(
            "GR-001",
            Guid.Empty,
            DateTime.UtcNow);

        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void AddItem_WithValidData_ShouldAddItem()
    {
        var receipt = CreateReceipt();
        var productId = Guid.NewGuid();

        receipt.AddItem(productId, 5);

        var item = Assert.Single(receipt.Items);

        Assert.Equal(productId, item.ProductId);
        Assert.Equal(5, item.Quantity);
    }

    [Fact]
    public void AddItem_WhenProductAlreadyExists_ShouldIncreaseQuantity()
    {
        var receipt = CreateReceipt();
        var productId = Guid.NewGuid();

        receipt.AddItem(productId, 5);
        receipt.AddItem(productId, 3);

        var item = Assert.Single(receipt.Items);

        Assert.Equal(8, item.Quantity);
    }

    [Fact]
    public void AddItem_WithEmptyProductId_ShouldThrow()
    {
        var receipt = CreateReceipt();

        var action = () => receipt.AddItem(
            Guid.Empty,
            1);

        Assert.Throws<DomainException>(action);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddItem_WithInvalidQuantity_ShouldThrow(
        int quantity)
    {
        var receipt = CreateReceipt();

        var action = () => receipt.AddItem(
            Guid.NewGuid(),
            quantity);

        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Confirm_WithItems_ShouldConfirmReceipt()
    {
        var receipt = CreateReceipt();

        receipt.AddItem(Guid.NewGuid(), 5);

        receipt.Confirm();

        Assert.Equal(
            GoodsReceiptStatus.Confirmed,
            receipt.Status);

        Assert.NotNull(receipt.ConfirmedAt);
    }

    [Fact]
    public void Confirm_WithoutItems_ShouldThrow()
    {
        var receipt = CreateReceipt();

        var action = receipt.Confirm;

        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Cancel_FromDraft_ShouldCancelReceipt()
    {
        var receipt = CreateReceipt();

        receipt.Cancel();

        Assert.Equal(
            GoodsReceiptStatus.Cancelled,
            receipt.Status);

        Assert.NotNull(receipt.CancelledAt);
    }

    [Fact]
    public void AddItem_AfterConfirmation_ShouldThrow()
    {
        var receipt = CreateReceipt();

        receipt.AddItem(Guid.NewGuid(), 1);
        receipt.Confirm();

        var action = () => receipt.AddItem(
            Guid.NewGuid(),
            1);

        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Confirm_AfterCancellation_ShouldThrow()
    {
        var receipt = CreateReceipt();

        receipt.Cancel();

        Assert.Throws<DomainException>(
            receipt.Confirm);
    }

    [Fact]
    public void Cancel_AfterConfirmation_ShouldThrow()
    {
        var receipt = CreateReceipt();

        receipt.AddItem(Guid.NewGuid(), 1);
        receipt.Confirm();

        Assert.Throws<DomainException>(
            receipt.Cancel);
    }

    private static GoodsReceipt CreateReceipt()
    {
        return new GoodsReceipt(
            "GR-001",
            Guid.NewGuid(),
            DateTime.UtcNow);
    }
}
