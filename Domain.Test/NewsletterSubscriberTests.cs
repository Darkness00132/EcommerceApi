using Domain.Entities.NewsletterAggregate;
using Domain.Exceptions;

namespace Domain.Test;

public class NewsletterSubscriberTests
{
    private const string ValidEmail = "user@example.com";

    [Fact]
    public void Constructor_WithValidEmail_ShouldInitializePropertiesCorrectly()
    {
        var subscriber = new NewsletterSubscriber("  user@example.com  ");

        Assert.NotEqual(Guid.Empty, subscriber.Id);
        Assert.Equal("user@example.com", subscriber.Email);
        Assert.True(subscriber.IsSubscribed);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyEmail_ShouldThrowDomainException(string? invalidEmail)
    {
        Assert.Throws<DomainException>(() => new NewsletterSubscriber(invalidEmail!));
    }

    [Fact]
    public void Unsubscribe_ShouldSetIsSubscribedToFalse()
    {
        var subscriber = new NewsletterSubscriber(ValidEmail);

        subscriber.Unsubscribe();

        Assert.False(subscriber.IsSubscribed);
    }

    [Fact]
    public void Subscribe_ShouldSetIsSubscribedToTrue()
    {
        var subscriber = new NewsletterSubscriber(ValidEmail);
        subscriber.Unsubscribe();

        subscriber.Subscribe();

        Assert.True(subscriber.IsSubscribed);
    }

    [Fact]
    public void ChangeEmail_WithValidEmail_ShouldUpdateEmail()
    {
        var subscriber = new NewsletterSubscriber(ValidEmail);

        subscriber.ChangeEmail("newuser@domain.com");

        Assert.Equal("newuser@domain.com", subscriber.Email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ChangeEmail_WithEmptyEmail_ShouldThrowDomainException(string? invalidEmail)
    {
        var subscriber = new NewsletterSubscriber(ValidEmail);

        Assert.Throws<DomainException>(() => subscriber.ChangeEmail(invalidEmail!));
    }
}
