using Domain.Common;
using Domain.Exceptions;

namespace Domain.Entities.NewsletterAggregate;

public sealed class NewsletterSubscriber : AggregateRoot
{
    public string Email { get; private set; } = null!;

    public bool IsSubscribed { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private NewsletterSubscriber() { }

    public NewsletterSubscriber(string email)
        : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email is required.");

        Email = email.Trim();
        IsSubscribed = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void Subscribe()
    {
        IsSubscribed = true;
    }

    public void Unsubscribe()
    {
        IsSubscribed = false;
    }

    public void ChangeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email is required.");

        Email = email.Trim();
    }
}