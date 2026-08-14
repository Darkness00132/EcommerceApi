using Domain.Entities.NewsletterAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.NewsletterAggregate;

public sealed class NewsletterSubscriberConfiguration : IEntityTypeConfiguration<NewsletterSubscriber>
{
    public void Configure(EntityTypeBuilder<NewsletterSubscriber> builder)
    {
        builder.Property(x => x.Email)
            .HasMaxLength(256);

        builder.HasIndex(x => x.Email)
            .IsUnique();
    }
}