using Domain.Entities.ProcurementAggregate;
using Domain.Exceptions;

namespace Domain.Test.ProcurementAggregate;

public class PurchaseOrderItemTests
{
    [Fact]
    public void Constructor_WithValidArguments_InitializesItem()
    {
        var item = new PurchaseOrderItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            5,
            10);

        Assert.Equal(5, item.OrderedQuantity);
        Assert.Equal(0, item.ReceivedQuantity);
        Assert.Equal(10, item.UnitCost);
    }

    [Fact]
    public void Receive_WithValidQuantity_IncreasesReceivedQuantity()
    {
        var item = new PurchaseOrderItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            10,
            5);

        item.Receive(4);

        Assert.Equal(4, item.ReceivedQuantity);
    }

    [Fact]
    public void Receive_ExceedingOrderedQuantity_ThrowsDomainException()
    {
        var item = new PurchaseOrderItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            10,
            5);

        var exception = Assert.Throws<DomainException>(() =>
            item.Receive(11));

        Assert.Equal(
            "Received quantity cannot exceed ordered quantity.",
            exception.Message);
    }
}
