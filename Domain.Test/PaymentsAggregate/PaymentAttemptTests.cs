using Domain.Entities.PaymentsAggregate;
using Domain.Enums;
using Domain.Exceptions;
using FluentAssertions;

namespace Domain.Test.PaymentsAggregate;

public class PaymentAttemptTests
{
    [Fact]
    public void A_Payment_Attempt_Is_Successful_When_It_Is_Completed()
    {
        // Arrange
        var attempt = CreateValidPaymentAttempt(Guid.NewGuid());

        // Act
        attempt.Complete("TXN-123");

        // Assert
        attempt.Status.Should().Be(PaymentAttemptStatus.Completed);
        attempt.TransactionId.Should().Be("TXN-123");
        attempt.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public void A_Payment_Attempt_Is_Failed_When_The_Payment_Cannot_Be_Completed()
    {
        // Arrange
        var attempt = CreateValidPaymentAttempt(Guid.NewGuid());

        // Act
        attempt.Fail();

        // Assert
        attempt.Status.Should().Be(PaymentAttemptStatus.Failed);
        attempt.FailedAt.Should().NotBeNull();
    }

    [Fact]
    public void A_Payment_Attempt_Is_Cancelled_When_The_Payment_Is_Cancelled()
    {
        // Arrange
        var attempt = CreateValidPaymentAttempt(Guid.NewGuid());

        // Act
        attempt.Cancel();

        // Assert
        attempt.Status.Should().Be(PaymentAttemptStatus.Cancelled);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void A_Payment_Attempt_Cannot_Be_Successful_Without_A_Transaction_Reference(
        string? transactionId)
    {
        // Arrange
        var attempt = CreateValidPaymentAttempt(Guid.NewGuid());

        // Act
        var act = () => attempt.Complete(transactionId!);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void A_Failed_Payment_Attempt_Cannot_Be_Successful_Again()
    {
        // Arrange
        var attempt = CreateValidPaymentAttempt(Guid.NewGuid());
        attempt.Fail();

        // Act
        var act = () => attempt.Complete("TXN-123");

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void A_Cancelled_Payment_Attempt_Cannot_Be_Successful_Again()
    {
        // Arrange
        var attempt = CreateValidPaymentAttempt(Guid.NewGuid());
        attempt.Cancel();

        // Act
        var act = () => attempt.Complete("TXN-123");

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void A_Successful_Payment_Attempt_Cannot_Be_Failed()
    {
        // Arrange
        var attempt = CreateValidPaymentAttempt(Guid.NewGuid());
        attempt.Complete("TXN-123");

        // Act
        var act = () => attempt.Fail();

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void A_Successful_Payment_Attempt_Cannot_Be_Cancelled()
    {
        // Arrange
        var attempt = CreateValidPaymentAttempt(Guid.NewGuid());
        attempt.Complete("TXN-123");

        // Act
        var act = () => attempt.Cancel();

        // Assert
        act.Should().Throw<DomainException>();
    }

    private PaymentAttempt CreateValidPaymentAttempt(Guid paymentId)
    {
        return new PaymentAttempt( paymentId, PaymentMethod.CashOnDelivery, 100.00m);
    }
}
