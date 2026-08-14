using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities.PaymentsAggregate;

public sealed class PaymentAttempt : Entity
{
    public Guid PaymentId { get; private set; }

    public Payment Payment { get; private set; } = null!;

    public PaymentMethod Method { get; private set; }

    public PaymentAttemptStatus Status { get; private set; }

    public decimal Amount { get; private set; }

    public string? TransactionId { get; private set; }

    public string? GatewayResponse { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public DateTime? FailedAt { get; private set; }

    private PaymentAttempt()
    {
    }

    public PaymentAttempt(
        Guid paymentId,
        PaymentMethod method,
        decimal amount)
        : base(Guid.NewGuid())
    {
        if (paymentId == Guid.Empty)
            throw new DomainException("Payment id is required.");

        if (amount <= 0)
            throw new DomainException("Payment attempt amount must be greater than zero.");

        PaymentId = paymentId;
        Method = method;
        Amount = amount;
        Status = PaymentAttemptStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void Complete(string transactionId, string? gatewayResponse = null)
    {
        if (Status != PaymentAttemptStatus.Pending)
            throw new DomainException("Only pending payment attempts can be completed.");

        if (string.IsNullOrWhiteSpace(transactionId))
            throw new DomainException("Transaction id is required.");

        Status = PaymentAttemptStatus.Completed;
        TransactionId = transactionId.Trim();
        GatewayResponse = string.IsNullOrWhiteSpace(gatewayResponse)
            ? null
            : gatewayResponse.Trim();

        CompletedAt = DateTime.UtcNow;
    }

    public void Fail(string? gatewayResponse = null)
    {
        if (Status != PaymentAttemptStatus.Pending)
            throw new DomainException("Only pending payment attempts can be failed.");

        Status = PaymentAttemptStatus.Failed;
        GatewayResponse = string.IsNullOrWhiteSpace(gatewayResponse)
            ? null
            : gatewayResponse.Trim();

        FailedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status != PaymentAttemptStatus.Pending)
            throw new DomainException("Only pending payment attempts can be cancelled.");

        Status = PaymentAttemptStatus.Cancelled;
    }
}