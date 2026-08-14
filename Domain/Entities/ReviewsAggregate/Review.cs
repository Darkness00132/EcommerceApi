using Domain.Common;
using Domain.Entities.Catalog;
using Domain.Entities.Identity;
using Domain.Exceptions;

namespace Domain.Entities.ReviewsAggregate;

public sealed class Review : AggregateRoot
{
    public Guid UserId { get; private set; }

    public AppUser User { get; private set; } = null!;

    public Guid ProductId { get; private set; }

    public Product Product { get; private set; } = null!;

    public int Rating { get; private set; }

    public string? Comment { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private Review() { }

    public Review(
        Guid userId,
        Guid productId,
        int rating,
        string? comment = null)
        : base(Guid.NewGuid())
    {
        if (userId == Guid.Empty)
            throw new DomainException("User id is required.");

        if (productId == Guid.Empty)
            throw new DomainException("Product id is required.");

        ValidateRating(rating);

        UserId = userId;
        ProductId = productId;
        Rating = rating;
        Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(int rating, string? comment = null)
    {
        ValidateRating(rating);

        Rating = rating;
        Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
    }

    private static void ValidateRating(int rating)
    {
        if (rating < 1 || rating > 5)
            throw new DomainException("Rating must be between 1 and 5.");
    }
}