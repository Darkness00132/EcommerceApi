using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Price)
                .HasPrecision(18, 2);

            builder.Property(p => p.SKU)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(p => p.SKU)
                .IsUnique();
            builder.Property(p => p.NameEn)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(p => p.NameAr)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(p => p.Brand)
                .HasMaxLength(100);
        }
    }
}
