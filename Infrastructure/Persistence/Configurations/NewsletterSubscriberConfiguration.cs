using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class NewsletterSubscriberConfiguration : IEntityTypeConfiguration<NewsletterSubscriber>
    {
        public void Configure(EntityTypeBuilder<NewsletterSubscriber> builder)
        {
            builder.Property(n => n.Email)
                .HasMaxLength(256);

            builder.HasIndex(n => n.Email)
                .IsUnique();
        }
    }
}
