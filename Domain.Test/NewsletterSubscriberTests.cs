using Domain.Entities.NewsletterAggregate;
using Domain.Exceptions;
using FluentAssertions;

namespace Domain.Test;

public class NewsletterSubscriberTests
{
    [Fact]
    public void A_Newsletter_Subscriber_Is_Subscribed_When_Created()
    {
        // Arrange & Act
        var subscriber = new NewsletterSubscriber("user@example.com");

        // Assert
        subscriber.IsSubscribed.Should().BeTrue();
    }

    [Fact]
    public void A_Newsletter_Subscriber_Cannot_Be_Created_Without_An_Email()
    {
        // Arrange & Act
        var act = () => new NewsletterSubscriber("");

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void A_Subscribed_Newsletter_Subscriber_Can_Unsubscribe()
    {
        // Arrange
        var subscriber = new NewsletterSubscriber("user@example.com");

        // Act
        subscriber.Unsubscribe();

        // Assert
        subscriber.IsSubscribed.Should().BeFalse();
    }

    [Fact]
    public void An_Unsubscribed_Newsletter_Subscriber_Can_Subscribe_Again()
    {
        // Arrange
        var subscriber = new NewsletterSubscriber("user@example.com");
        subscriber.Unsubscribe();

        // Act
        subscriber.Subscribe();

        // Assert
        subscriber.IsSubscribed.Should().BeTrue();
    }

    [Fact]
    public void A_Newsletter_Subscriber_Can_Change_Their_Email()
    {
        // Arrange
        var subscriber = new NewsletterSubscriber("old@example.com");

        // Act
        subscriber.ChangeEmail("new@example.com");

        // Assert
        subscriber.Email.Should().Be("new@example.com");
    }

    [Fact]
    public void A_Newsletter_Subscriber_Cannot_Change_Their_Email_To_An_Empty_Email()
    {
        // Arrange
        var subscriber = new NewsletterSubscriber("old@example.com");

        // Act
        var act = () => subscriber.ChangeEmail("");

        // Assert
        act.Should().Throw<DomainException>();
    }
}
