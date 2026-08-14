using Domain.Entities.ReviewsAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.ReviewsAggregate;

public sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.Property(x => x.Comment)
            .HasMaxLength(1000);

        builder.HasIndex(x => new { x.UserId, x.ProductId })
            .IsUnique();
    }
}