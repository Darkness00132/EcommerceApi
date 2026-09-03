using Domain.Entities.PaymentsAggregate;
using Domain.Enums;
using Domain.Exceptions;
using FluentAssertions;

namespace Domain.Test.PaymentsAggregate;

public class PaymentTests
{
    [Fact]
    public void Payment_Is_Pending_When_Created()
    {
        // Arrange & Act
        var payment = CreateValidPayment();

        // Assert
        payment.Status.Should().Be(PaymentStatus.Pending);
    }

    [Fact]
    public void Payment_Is_Paid_When_Payment_Is_Confirmed()
    {
        // Arrange
        var payment = CreateValidPayment();

        // Act
        payment.MarkAsPaid();

        // Assert
        payment.Status.Should().Be(PaymentStatus.Paid);
        payment.PaidAt.Should().NotBeNull();
    }

    [Fact]
    public void Payment_Remains_Paid_When_Confirmed_Again()
    {
        // Arrange
        var payment = CreateValidPayment();
        payment.MarkAsPaid();
        var paidAt = payment.PaidAt;

        // Act
        payment.MarkAsPaid();

        // Assert
        payment.Status.Should().Be(PaymentStatus.Paid);
        payment.PaidAt.Should().Be(paidAt);
    }

    [Fact]
    public void Payment_Marked_As_Failed_When_Payment_Fails()
    {
        // Arrange
        var payment = CreateValidPayment();

        // Act
        payment.MarkAsFailed();

        // Assert
        payment.Status.Should().Be(PaymentStatus.Failed);
    }

    [Fact]
    public void Payment_Is_Cancelled_When_Payment_Is_Cancelled()
    {
        // Arrange
        var payment = CreateValidPayment();

        // Act
        payment.Cancel();

        // Assert
        payment.Status.Should().Be(PaymentStatus.Cancelled);
    }

    [Fact]
    public void Payment_Is_Refunded_When_Paid_Payment_Is_Refunded()
    {
        // Arrange
        var payment = CreateValidPayment();
        payment.MarkAsPaid();

        // Act
        payment.Refund();

        // Assert
        payment.Status.Should().Be(PaymentStatus.Refunded);
        payment.RefundedAt.Should().NotBeNull();
    }

    [Fact]
    public void Payment_Cannot_Be_Refunded_Before_Payment_Is_Paid()
    {
        // Arrange
        var payment = CreateValidPayment();

        // Act
        var act = () => payment.Refund();

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Payment_Cannot_Be_Cancelled_After_Payment_Is_Paid()
    {
        // Arrange
        var payment = CreateValidPayment();
        payment.MarkAsPaid();

        // Act
        var act = () => payment.Cancel();

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Payment_Cannot_Be_Marked_As_Failed_After_Payment_Is_Paid()
    {
        // Arrange
        var payment = CreateValidPayment();
        payment.MarkAsPaid();

        // Act
        var act = () => payment.MarkAsFailed();

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Payment_Cannot_Be_Marked_As_Paid_After_Payment_Is_Cancelled()
    {
        // Arrange
        var payment = CreateValidPayment();
        payment.Cancel();

        // Act
        var act = () => payment.MarkAsPaid();

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Payment_Cannot_Be_Marked_As_Paid_After_Payment_Is_Refunded()
    {
        // Arrange
        var payment = CreateValidPayment();
        payment.MarkAsPaid();
        payment.Refund();

        // Act
        var act = () => payment.MarkAsPaid();

        // Assert
        act.Should().Throw<DomainException>();
    }
    private Payment CreateValidPayment()
    {
        return new Payment(Guid.NewGuid(), 100.00m);
    }
}
