using Domain.Entities.PromotionsAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.PromotionsAggregate;

public sealed class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
{
    public void Configure(EntityTypeBuilder<PromoCode> builder)
    {
        builder.Property(x => x.Code)
            .HasMaxLength(50);

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.Property(x => x.Value)
            .HasPrecision(18, 4);

        builder.Property(x => x.MinimumOrder)
            .HasPrecision(18, 2);

        builder.OwnsOne(x => x.ValidityPeriod);

        builder.Property(x => x.RowVersion)
            .IsRowVersion();
    }
}