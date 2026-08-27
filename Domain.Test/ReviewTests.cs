using Domain.Entities.ReviewsAggregate;
using Domain.Exceptions;

namespace Domain.Tests;

public class ReviewTests
{
    [Fact]
    public void Constructor_WithValidArguments_InitializesReview()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        // Act
        var review = new Review(
            userId,
            productId,
            5,
            "  Great product  ");

        // Assert
        Assert.NotEqual(Guid.Empty, review.Id);
        Assert.Equal(userId, review.UserId);
        Assert.Equal(productId, review.ProductId);
        Assert.Equal(5, review.Rating);
        Assert.Equal("Great product", review.Comment);
        Assert.True(
            (DateTime.UtcNow - review.CreatedAt).TotalSeconds < 1);
    }

    [Fact]
    public void Constructor_WithEmptyUserId_ThrowsDomainException()
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            new Review(
                Guid.Empty,
                Guid.NewGuid(),
                5));

        Assert.Equal(
            "User id is required.",
            exception.Message);
    }

    [Fact]
    public void Constructor_WithEmptyProductId_ThrowsDomainException()
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            new Review(
                Guid.NewGuid(),
                Guid.Empty,
                5));

        Assert.Equal(
            "Product id is required.",
            exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    [InlineData(100)]
    public void Constructor_WithInvalidRating_ThrowsDomainException(
        int rating)
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            new Review(
                Guid.NewGuid(),
                Guid.NewGuid(),
                rating));

        Assert.Equal(
            "Rating must be between 1 and 5.",
            exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyComment_SetsCommentToNull(
        string? comment)
    {
        // Act
        var review = new Review(
            Guid.NewGuid(),
            Guid.NewGuid(),
            5,
            comment);

        // Assert
        Assert.Null(review.Comment);
    }

    [Fact]
    public void Constructor_WithCommentExceedingMaximumLength_ThrowsDomainException()
    {
        // Arrange
        var comment = new string('A', 1001);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            new Review(
                Guid.NewGuid(),
                Guid.NewGuid(),
                5,
                comment));

        Assert.Equal(
            "Comment cannot exceed 1000 characters.",
            exception.Message);
    }

    [Fact]
    public void Update_WithValidArguments_UpdatesReview()
    {
        // Arrange
        var review = new Review(
            Guid.NewGuid(),
            Guid.NewGuid(),
            3,
            "Old comment");

        // Act
        review.Update(
            5,
            "  New comment  ");

        // Assert
        Assert.Equal(5, review.Rating);
        Assert.Equal("New comment", review.Comment);
        Assert.NotNull(review.UpdatedAt);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    public void Update_WithInvalidRating_ThrowsDomainException(
        int rating)
    {
        // Arrange
        var review = new Review(
            Guid.NewGuid(),
            Guid.NewGuid(),
            5);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            review.Update(rating));

        Assert.Equal(
            "Rating must be between 1 and 5.",
            exception.Message);
    }

    [Fact]
    public void Update_WithCommentExceedingMaximumLength_ThrowsDomainException()
    {
        // Arrange
        var review = new Review(
            Guid.NewGuid(),
            Guid.NewGuid(),
            5);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            review.Update(
                5,
                new string('A', 1001)));

        Assert.Equal(
            "Comment cannot exceed 1000 characters.",
            exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_WithEmptyComment_SetsCommentToNull(
        string? comment)
    {
        // Arrange
        var review = new Review(
            Guid.NewGuid(),
            Guid.NewGuid(),
            5,
            "Test");

        // Act
        review.Update(4, comment);

        // Assert
        Assert.Equal(4, review.Rating);
        Assert.Null(review.Comment);
    }
}
