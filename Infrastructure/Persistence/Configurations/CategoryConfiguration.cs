using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(c => c.NameEn)
                .HasMaxLength(100);

            builder.Property(c => c.NameAr)
                .HasMaxLength(100);

            builder.Property(c => c.DescriptionEn)
                .HasMaxLength(500);

            builder.Property(c => c.DescriptionAr)
                .HasMaxLength(500);

            builder.Property(c => c.ImageKey)
                .HasMaxLength(500);
        }
    }
}