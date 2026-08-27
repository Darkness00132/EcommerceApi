using Domain.Entities.PaymentsAggregate;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Test.PaymentsAggregate;

public sealed class PaymentTests
{
    [Fact]
    public void Constructor_WithValidArguments_InitializesPendingPayment()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var amount = 150.00m;

        // Act
        var payment = new Payment(orderId, amount);

        // Assert
        Assert.NotEqual(Guid.Empty, payment.Id);
        Assert.Equal(orderId, payment.OrderId);
        Assert.Equal(amount, payment.Amount);
        Assert.Equal(PaymentStatus.Pending, payment.Status);
        Assert.True((DateTime.UtcNow - payment.CreatedAt).TotalSeconds < 1);
        Assert.Null(payment.PaidAt);
        Assert.Null(payment.RefundedAt);
        Assert.Empty(payment.Attempts);
    }

    [Fact]
    public void Constructor_WithEmptyOrderId_ThrowsDomainException()
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Payment(Guid.Empty, 100m));
        Assert.Equal("Order id is required.", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Constructor_WithInvalidAmount_ThrowsDomainException(decimal invalidAmount)
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Payment(Guid.NewGuid(), invalidAmount));
        Assert.Equal("Payment amount must be greater than zero.", exception.Message);
    }

    [Fact]
    public void AddAttempt_WhenPaymentIsPending_AddsAttemptToCollection()
    {
        // Arrange
        var payment = new Payment(Guid.NewGuid(), 200m);

        // Act
        payment.AddAttempt(PaymentMethod.Card, 200m);

        // Assert
        Assert.Single(payment.Attempts);
        var attempt = Assert.Single(payment.Attempts, a => a.Method == PaymentMethod.Card);
        Assert.Equal(payment.Id, attempt.PaymentId);
        Assert.Equal(200m, attempt.Amount);
    }

    [Theory]
    [InlineData(PaymentStatus.Paid)]
    [InlineData(PaymentStatus.Refunded)]
    [InlineData(PaymentStatus.Cancelled)]
    public void AddAttempt_WhenPaymentIsInTerminalState_ThrowsDomainException(PaymentStatus terminalStatus)
    {
        // Arrange
        var payment = new Payment(Guid.NewGuid(), 100m);
        SetPaymentStatus(payment, terminalStatus);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => payment.AddAttempt(PaymentMethod.Card, 100m));
        Assert.Equal("Cannot add payment attempt for paid, refunded, or cancelled payment.", exception.Message);
    }

    [Fact]
    public void MarkAsPaid_WhenPending_TransitionsToPaidAndSetsPaidAt()
    {
        // Arrange
        var payment = new Payment(Guid.NewGuid(), 100m);

        // Act
        payment.MarkAsPaid();

        // Assert
        Assert.Equal(PaymentStatus.Paid, payment.Status);
        Assert.NotNull(payment.PaidAt);
        Assert.True((DateTime.UtcNow - payment.PaidAt!.Value).TotalSeconds < 1);
    }

    [Fact]
    public void MarkAsPaid_WhenAlreadyPaid_IsIdempotent()
    {
        // Arrange
        var payment = new Payment(Guid.NewGuid(), 100m);
        payment.MarkAsPaid();
        var initialPaidAt = payment.PaidAt;

        // Act
        payment.MarkAsPaid();

        // Assert
        Assert.Equal(PaymentStatus.Paid, payment.Status);
        Assert.Equal(initialPaidAt, payment.PaidAt);
    }

    [Theory]
    [InlineData(PaymentStatus.Refunded)]
    [InlineData(PaymentStatus.Cancelled)]
    public void MarkAsPaid_WhenRefundedOrCancelled_ThrowsDomainException(PaymentStatus invalidStatus)
    {
        // Arrange
        var payment = new Payment(Guid.NewGuid(), 100m);
        SetPaymentStatus(payment, invalidStatus);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => payment.MarkAsPaid());
        Assert.Equal("Refunded or cancelled payment cannot be marked as paid.", exception.Message);
    }

    [Fact]
    public void Refund_WhenPaid_TransitionsToRefundedAndSetsRefundedAt()
    {
        // Arrange
        var payment = new Payment(Guid.NewGuid(), 100m);
        payment.MarkAsPaid();

        // Act
        payment.Refund();

        // Assert
        Assert.Equal(PaymentStatus.Refunded, payment.Status);
        Assert.NotNull(payment.RefundedAt);
        Assert.True((DateTime.UtcNow - payment.RefundedAt!.Value).TotalSeconds < 1);
    }

    [Fact]
    public void Refund_WhenNotPaid_ThrowsDomainException()
    {
        // Arrange
        var payment = new Payment(Guid.NewGuid(), 100m);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => payment.Refund());
        Assert.Equal("Only paid payments can be refunded.", exception.Message);
    }

    private static void SetPaymentStatus(Payment payment, PaymentStatus status)
    {
        switch (status) {
            case PaymentStatus.Paid:
                payment.MarkAsPaid();
                break;
            case PaymentStatus.Failed:
                payment.MarkAsFailed();
                break;
            case PaymentStatus.Cancelled:
                payment.Cancel();
                break;
            case PaymentStatus.Refunded:
                payment.MarkAsPaid();
                payment.Refund();
                break;
        }
    }
}
