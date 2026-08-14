using Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Catalog;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(x => x.NameEn)
            .HasMaxLength(200);

        builder.Property(x => x.NameAr)
            .HasMaxLength(200);

        builder.Property(x => x.SKU)
            .HasMaxLength(100);

        builder.HasIndex(x => x.SKU)
            .IsUnique();

        builder.Property(x => x.DescriptionEn)
            .HasMaxLength(2000);

        builder.Property(x => x.DescriptionAr)
            .HasMaxLength(2000);

        builder.Property(x => x.Price)
            .HasPrecision(18, 2);

        builder.Property(x => x.RowVersion)
            .IsRowVersion();
    }
}