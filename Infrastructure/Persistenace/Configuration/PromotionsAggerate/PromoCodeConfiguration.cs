using Domain.Entities.PromotionsAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.PromotionsAggregate;

public sealed class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
{
    public void Configure(EntityTypeBuilder<PromoCode> builder)
    {
        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.OwnsOne(x => x.ValidityPeriod);
    }
}
