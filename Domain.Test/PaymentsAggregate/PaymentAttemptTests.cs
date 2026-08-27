using Domain.Entities.PaymentsAggregate;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Test.PaymentsAggregate;

public sealed class PaymentAttemptTests
{
    [Fact]
    public void Constructor_WithValidArguments_InitializesPendingAttempt()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var amount = 99.99m;

        // Act
        var attempt = new PaymentAttempt(paymentId, PaymentMethod.Card, amount);

        // Assert
        Assert.NotEqual(Guid.Empty, attempt.Id);
        Assert.Equal(paymentId, attempt.PaymentId);
        Assert.Equal(PaymentMethod.Card, attempt.Method);
        Assert.Equal(amount, attempt.Amount);
        Assert.Equal(PaymentAttemptStatus.Pending, attempt.Status);
        Assert.Null(attempt.TransactionId);
        Assert.Null(attempt.GatewayResponse);
        Assert.True((DateTime.UtcNow - attempt.CreatedAt).TotalSeconds < 1);
        Assert.Null(attempt.CompletedAt);
        Assert.Null(attempt.FailedAt);
    }

    [Fact]
    public void Complete_WhenPending_SetsTransactionIdAndCompletedAt()
    {
        // Arrange
        var attempt = new PaymentAttempt(Guid.NewGuid(), PaymentMethod.Card, 50m);

        // Act
        attempt.Complete("  TXN_12345  ", "  {\"code\":200}  ");

        // Assert
        Assert.Equal(PaymentAttemptStatus.Completed, attempt.Status);
        Assert.Equal("TXN_12345", attempt.TransactionId);
        Assert.Equal("{\"code\":200}", attempt.GatewayResponse);
        Assert.NotNull(attempt.CompletedAt);
        Assert.True((DateTime.UtcNow - attempt.CompletedAt!.Value).TotalSeconds < 1);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Complete_WithInvalidTransactionId_ThrowsDomainException(string? invalidTxnId)
    {
        // Arrange
        var attempt = new PaymentAttempt(Guid.NewGuid(), PaymentMethod.Card, 50m);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => attempt.Complete(invalidTxnId!));
        Assert.Equal("Transaction id is required.", exception.Message);
    }

    [Fact]
    public void Complete_WhenNotPending_ThrowsDomainException()
    {
        // Arrange
        var attempt = new PaymentAttempt(Guid.NewGuid(), PaymentMethod.Card, 50m);
        attempt.Complete("TXN_1");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => attempt.Complete("TXN_2"));
        Assert.Equal("Only pending payment attempts can be completed.", exception.Message);
    }

    [Fact]
    public void Fail_WhenPending_SetsStatusAndFailedAt()
    {
        // Arrange
        var attempt = new PaymentAttempt(Guid.NewGuid(), PaymentMethod.Card, 50m);

        // Act
        attempt.Fail("Insufficient funds");

        // Assert
        Assert.Equal(PaymentAttemptStatus.Failed, attempt.Status);
        Assert.Equal("Insufficient funds", attempt.GatewayResponse);
        Assert.NotNull(attempt.FailedAt);
        Assert.True((DateTime.UtcNow - attempt.FailedAt!.Value).TotalSeconds < 1);
    }

    [Fact]
    public void Fail_WhenNotPending_ThrowsDomainException()
    {
        // Arrange
        var attempt = new PaymentAttempt(Guid.NewGuid(), PaymentMethod.Card, 50m);
        attempt.Fail();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => attempt.Fail());
        Assert.Equal("Only pending payment attempts can be failed.", exception.Message);
    }
}
