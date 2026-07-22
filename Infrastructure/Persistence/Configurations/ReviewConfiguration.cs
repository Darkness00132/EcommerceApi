using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.Property(r => r.Comment)
                .HasMaxLength(1000);

            builder.ToTable(t => t.HasCheckConstraint(
                "CK_Review_Rating", 
                $"[{nameof(Review.Rating)}] >= 1 AND [{nameof(Review.Rating)}] <= 5"
            ));

            builder.HasIndex(r => new { r.UserId, r.ProductId })
                .IsUnique();
        }
    }
}