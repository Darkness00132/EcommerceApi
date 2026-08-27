using Domain.Entities.ProcurementAggregate;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Test.ProcurementAggregate;

public class PurchaseOrderTests
{
    [Fact]
    public void Constructor_WithValidArguments_InitializesDraftOrder()
    {
        var supplierId = Guid.NewGuid();
        var orderDate = DateOnly.FromDateTime(DateTime.UtcNow);

        var order = new PurchaseOrder(
            " PO-1001 ",
            supplierId,
            orderDate);

        Assert.Equal("PO-1001", order.Number);
        Assert.Equal(supplierId, order.SupplierId);
        Assert.Equal(PurchaseOrderStatus.Draft, order.Status);

        Assert.Equal(0, order.Subtotal);
        Assert.Equal(0, order.Total);
        Assert.Empty(order.Items);
    }

    [Fact]
    public void Constructor_WithEmptySupplierId_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new PurchaseOrder(
                "PO-1001",
                Guid.Empty,
                DateOnly.FromDateTime(DateTime.UtcNow)));

        Assert.Equal(
            "Supplier id is required.",
            exception.Message);
    }

    [Fact]
    public void AddItem_WithValidData_AddsItemAndCalculatesSubtotal()
    {
        var order = CreateOrder();

        order.AddItem(Guid.NewGuid(), 5, 10);

        Assert.Single(order.Items);
        Assert.Equal(50, order.Subtotal);
        Assert.Equal(50, order.Total);
    }

    [Fact]
    public void AddItem_WithExistingProduct_IncreasesQuantity()
    {
        var order = CreateOrder();
        var productId = Guid.NewGuid();

        order.AddItem(productId, 5, 10);
        order.AddItem(productId, 2, 10);

        var item = Assert.Single(order.Items);

        Assert.Equal(7, item.OrderedQuantity);
        Assert.Equal(70, order.Subtotal);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddItem_WithInvalidQuantity_ThrowsDomainException(
        int quantity)
    {
        var order = CreateOrder();

        var exception = Assert.Throws<DomainException>(() =>
            order.AddItem(Guid.NewGuid(), quantity, 10));

        Assert.Equal(
            "Ordered quantity must be greater than zero.",
            exception.Message);
    }

    [Fact]
    public void SetCosts_RecalculatesTotal()
    {
        var order = CreateOrder();

        order.AddItem(Guid.NewGuid(), 5, 10);

        order.SetCosts(20, 5);

        Assert.Equal(50, order.Subtotal);
        Assert.Equal(75, order.Total);
    }

    [Fact]
    public void SubmitForApproval_WithItems_ChangesStatus()
    {
        var order = CreateOrder();
        order.AddItem(Guid.NewGuid(), 1, 10);

        order.SubmitForApproval();

        Assert.Equal(
            PurchaseOrderStatus.PendingApproval,
            order.Status);
    }

    [Fact]
    public void SubmitForApproval_WithoutItems_ThrowsDomainException()
    {
        var order = CreateOrder();

        var exception = Assert.Throws<DomainException>(
            order.SubmitForApproval);

        Assert.Equal(
            "Cannot submit purchase order without items.",
            exception.Message);
    }

    [Fact]
    public void Approve_WhenPendingApproval_ChangesStatus()
    {
        var order = CreateApprovedCandidate();

        order.Approve();

        Assert.Equal(
            PurchaseOrderStatus.Approved,
            order.Status);

        Assert.NotNull(order.ApprovedAt);
    }

    [Fact]
    public void Complete_WhenApproved_ChangesStatus()
    {
        var order = CreateApprovedOrder();

        order.Complete();

        Assert.Equal(
            PurchaseOrderStatus.Completed,
            order.Status);

        Assert.NotNull(order.CompletedAt);
    }

    [Fact]
    public void Cancel_WhenCompleted_ThrowsDomainException()
    {
        var order = CreateApprovedOrder();
        order.Complete();

        var exception = Assert.Throws<DomainException>(
            order.Cancel);

        Assert.Equal(
            "Completed purchase orders cannot be cancelled.",
            exception.Message);
    }

    private static PurchaseOrder CreateOrder()
    {
        return new PurchaseOrder(
            "PO-1001",
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.UtcNow));
    }

    private static PurchaseOrder CreateApprovedCandidate()
    {
        var order = CreateOrder();
        order.AddItem(Guid.NewGuid(), 1, 10);
        order.SubmitForApproval();

        return order;
    }

    private static PurchaseOrder CreateApprovedOrder()
    {
        var order = CreateApprovedCandidate();
        order.Approve();

        return order;
    }
}
