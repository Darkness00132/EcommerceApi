using Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Catalog;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.Property(x => x.NameEn)
            .HasMaxLength(100);

        builder.Property(x => x.NameAr)
            .HasMaxLength(100);

        builder.Property(x => x.DescriptionEn)
            .HasMaxLength(500);

        builder.Property(x => x.DescriptionAr)
            .HasMaxLength(500);

        builder.Property(x => x.ImageKey)
            .HasMaxLength(500);
    }
}