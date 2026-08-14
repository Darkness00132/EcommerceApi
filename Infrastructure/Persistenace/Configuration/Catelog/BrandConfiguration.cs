using Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Catalog;

public sealed class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.Property(x => x.NameEn)
            .HasMaxLength(100);

        builder.Property(x => x.NameAr)
            .HasMaxLength(100);

        builder.HasIndex(x => x.NameEn)
            .IsUnique();

        builder.HasIndex(x => x.NameAr)
            .IsUnique();
    }
}