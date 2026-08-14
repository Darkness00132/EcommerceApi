using Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Catalog;

public sealed class DiscountConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(100);

        builder.Property(x => x.Value)
            .HasPrecision(18, 4);

        builder.OwnsOne(x => x.ValidityPeriod);
    }
}