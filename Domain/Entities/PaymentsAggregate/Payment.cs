using Domain.Common;
using Domain.Entities.OrdersAggregate;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities.PaymentsAggregate;

public sealed class Payment : AggregateRoot
{
    public Guid OrderId { get; private set; }

    public Order Order { get; private set; } = null!;

    public PaymentStatus Status { get; private set; }

    public decimal Amount { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? PaidAt { get; private set; }

    public DateTime? RefundedAt { get; private set; }

    public byte[] RowVersion { get; private set; } = [];

    public ICollection<PaymentAttempt> Attempts { get; private set; } = new List<PaymentAttempt>();

    private Payment() { }

    public Payment(Guid orderId, decimal amount)
        : base(Guid.NewGuid())
    {
        if (orderId == Guid.Empty)
            throw new DomainException("Order id is required.");

        if (amount <= 0)
            throw new DomainException("Payment amount must be greater than zero.");

        OrderId = orderId;
        Amount = amount;
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddAttempt(PaymentMethod method, decimal amount)
    {
        if (Status is PaymentStatus.Paid or PaymentStatus.Refunded or PaymentStatus.Cancelled)
            throw new DomainException("Cannot add payment attempt for paid, refunded, or cancelled payment.");

        if (amount <= 0)
            throw new DomainException("Payment attempt amount must be greater than zero.");

        Attempts.Add(new PaymentAttempt(
            paymentId: Id,
            method: method,
            amount: amount));
    }

    public void MarkAsPaid()
    {
        if (Status == PaymentStatus.Paid)
            return;

        if (Status is PaymentStatus.Refunded or PaymentStatus.Cancelled)
            throw new DomainException("Refunded or cancelled payment cannot be marked as paid.");

        Status = PaymentStatus.Paid;
        PaidAt = DateTime.UtcNow;
    }

    public void MarkAsFailed()
    {
        if (Status == PaymentStatus.Paid)
            throw new DomainException("Paid payment cannot be marked as failed.");

        if (Status == PaymentStatus.Refunded)
            throw new DomainException("Refunded payment cannot be marked as failed.");

        Status = PaymentStatus.Failed;
    }

    public void Cancel()
    {
        if (Status == PaymentStatus.Paid)
            throw new DomainException("Paid payment cannot be cancelled.");

        if (Status == PaymentStatus.Refunded)
            throw new DomainException("Refunded payment cannot be cancelled.");

        Status = PaymentStatus.Cancelled;
    }

    public void Refund()
    {
        if (Status != PaymentStatus.Paid)
            throw new DomainException("Only paid payments can be refunded.");

        Status = PaymentStatus.Refunded;
        RefundedAt = DateTime.UtcNow;
    }
}