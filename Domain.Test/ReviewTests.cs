using Domain.Entities.ReviewsAggregate;
using Domain.Exceptions;
using FluentAssertions;

namespace Domain.Tests;

public class ReviewTests
{
    [Fact]
    public void A_Review_Can_Be_Created_With_Valid_Information()
    {
        // Arrange & Act
        var review = CreateValidReview();

        // Assert
        review.Rating.Should().Be(5);
        review.Comment.Should().Be("Great product!");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void A_Review_Cannot_Have_A_Rating_Outside_The_Allowed_Range(int rating)
    {
        // Arrange & Act
        var act = () => new Review(
            Guid.NewGuid(),
            Guid.NewGuid(),
            rating);

        // Assert
        act.Should()
            .Throw<DomainException>()
            .WithMessage("Rating must be between 1 and 5.");
    }

    [Fact]
    public void A_Review_Can_Be_Updated()
    {
        // Arrange
        var review = CreateValidReview();

        // Act
        review.Update(4, "Good product!");

        // Assert
        review.Rating.Should().Be(4);
        review.Comment.Should().Be("Good product!");
    }

    [Fact]
    public void A_Review_Cannot_Be_Updated_With_An_Invalid_Rating()
    {
        // Arrange
        var review = CreateValidReview();

        // Act
        var act = () => review.Update(6);

        // Assert
        act.Should()
            .Throw<DomainException>()
            .WithMessage("Rating must be between 1 and 5.");
    }

    private static Review CreateValidReview()
    {
        return new Review(
            Guid.NewGuid(),
            Guid.NewGuid(),
            5,
            "Great product!");
    }
}
