using Domain.Entities.ProcurementAggregate;
using Domain.Enums;
using Domain.Exceptions;
using FluentAssertions;

namespace Domain.Test.ProcurementAggregate;

public class PurchaseOrderTests
{
    private readonly DateOnly Today = DateOnly.FromDateTime(DateTime.UtcNow);

    [Fact]
    public void A_Purchase_Order_Is_Draft_When_Created()
    {
        // Arrange & Act
        var purchaseOrder = CreateValidPurchase();

        // Assert
        purchaseOrder.Status.Should().Be(PurchaseOrderStatus.Draft);
    }

    [Fact]
    public void A_Purchase_Order_Records_The_Product_Quantity_And_Cost_When_An_Item_Is_Added()
    {
        // Arrange
        var purchaseOrder = CreateValidPurchase();
        var productId = Guid.NewGuid();

        // Act
        purchaseOrder.AddItem(productId, 10, 50m);

        // Assert
        purchaseOrder.Items.Should().ContainSingle(item =>
            item.ProductId == productId &&
            item.OrderedQuantity == 10 &&
            item.UnitCost == 50m);
    }

    [Fact]
    public void A_Purchase_Order_Increases_The_Ordered_Quantity_When_The_Same_Product_Is_Added_Again()
    {
        // Arrange
        var purchaseOrder = CreateValidPurchase();
        var productId = Guid.NewGuid();

        purchaseOrder.AddItem(productId, 10, 50m);

        // Act
        purchaseOrder.AddItem(productId, 5, 50m);

        // Assert
        purchaseOrder.Items.Should().ContainSingle();
        purchaseOrder.Items.Single().OrderedQuantity.Should().Be(15);
    }

    [Theory]
    [InlineData(10, 50, 500)]
    [InlineData(3, 125.50, 376.50)]
    [InlineData(20, 25.75, 515)]
    public void A_Purchase_Order_Calculates_The_Subtotal_From_Its_Items(
        int quantity,
        decimal unitCost,
        decimal expectedSubtotal)
    {
        // Arrange
        var purchaseOrder = CreateValidPurchase();

        // Act
        purchaseOrder.AddItem(Guid.NewGuid(), quantity, unitCost);

        // Assert
        purchaseOrder.Subtotal.Should().Be(expectedSubtotal);
    }

    [Theory]
    [InlineData(500, 50, 25, 575)]
    [InlineData(376.50, 37.65, 20, 434.15)]
    [InlineData(1000, 0, 100, 1100)]
    public void A_Purchase_Order_Includes_Tax_And_Shipping_In_The_Total(
        decimal subtotal,
        decimal taxAmount,
        decimal shippingCost,
        decimal expectedTotal)
    {
        // Arrange
        var purchaseOrder = CreateValidPurchase();
        purchaseOrder.AddItem(Guid.NewGuid(), 1, subtotal);

        // Act
        purchaseOrder.SetCosts(taxAmount, shippingCost);

        // Assert
        purchaseOrder.Total.Should().Be(expectedTotal);
    }

    [Fact]
    public void A_Purchase_Order_Cannot_Be_Submitted_For_Approval_Without_Items()
    {
        // Arrange
        var purchaseOrder = CreateValidPurchase();

        // Act
        var act = () => purchaseOrder.SubmitForApproval();

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void A_Purchase_Order_Becomes_Pending_Approval_When_Submitted_With_Items()
    {
        // Arrange
        var purchaseOrder = CreateValidPurchase();
        purchaseOrder.AddItem(Guid.NewGuid(), 10, 50m);

        // Act
        purchaseOrder.SubmitForApproval();

        // Assert
        purchaseOrder.Status.Should().Be(PurchaseOrderStatus.PendingApproval);
    }

    [Fact]
    public void A_Purchase_Order_Becomes_Approved_After_Approval()
    {
        // Arrange
        var purchaseOrder = CreateValidPurchase();
        purchaseOrder.AddItem(Guid.NewGuid(), 10, 50m);
        purchaseOrder.SubmitForApproval();

        // Act
        purchaseOrder.Approve();

        // Assert
        purchaseOrder.Status.Should().Be(PurchaseOrderStatus.Approved);
        purchaseOrder.ApprovedAt.Should().NotBeNull();
    }

    [Fact]
    public void A_Purchase_Order_Can_Be_Marked_As_Partially_Received_After_Approval()
    {
        // Arrange
        var purchaseOrder = CreateApprovedPurchase();

        // Act
        purchaseOrder.MarkAsPartiallyReceived();

        // Assert
        purchaseOrder.Status.Should().Be(PurchaseOrderStatus.PartiallyReceived);
    }

    [Fact]
    public void A_Purchase_Order_Can_Be_Completed_After_Approval()
    {
        // Arrange
        var purchaseOrder = CreateApprovedPurchase();

        // Act
        purchaseOrder.Complete();

        // Assert
        purchaseOrder.Status.Should().Be(PurchaseOrderStatus.Completed);
        purchaseOrder.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public void A_Purchase_Order_Can_Be_Completed_After_Partial_Receipt()
    {
        // Arrange
        var purchaseOrder = CreateApprovedPurchase();
        purchaseOrder.MarkAsPartiallyReceived();

        // Act
        purchaseOrder.Complete();

        // Assert
        purchaseOrder.Status.Should().Be(PurchaseOrderStatus.Completed);
        purchaseOrder.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public void A_Purchase_Order_Can_Be_Cancelled_Before_Completion()
    {
        // Arrange
        var purchaseOrder = CreateValidPurchase();

        // Act
        purchaseOrder.Cancel();

        // Assert
        purchaseOrder.Status.Should().Be(PurchaseOrderStatus.Cancelled);
        purchaseOrder.CancelledAt.Should().NotBeNull();
    }

    [Fact]
    public void A_Completed_Purchase_Order_Cannot_Be_Cancelled()
    {
        // Arrange
        var purchaseOrder = CreateApprovedPurchase();
        purchaseOrder.Complete();

        // Act
        var act = () => purchaseOrder.Cancel();

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void A_Cancelled_Purchase_Order_Cannot_Be_Cancelled_Again()
    {
        // Arrange
        var purchaseOrder = CreateValidPurchase();
        purchaseOrder.Cancel();

        // Act
        var act = () => purchaseOrder.Cancel();

        // Assert
        act.Should().Throw<DomainException>();
    }

    private PurchaseOrder CreateValidPurchase()
    {
        return new PurchaseOrder(
            "PO-001",
            Guid.NewGuid(),
            Today,
            Today.AddDays(7),
            notes: "Test purchase order");
    }

    private PurchaseOrder CreateApprovedPurchase()
    {
        var purchaseOrder = CreateValidPurchase();

        purchaseOrder.AddItem(Guid.NewGuid(), 10, 50m);
        purchaseOrder.SubmitForApproval();
        purchaseOrder.Approve();

        return purchaseOrder;
    }
}
