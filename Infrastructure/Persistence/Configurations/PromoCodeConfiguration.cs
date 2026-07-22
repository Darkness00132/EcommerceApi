using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
    {
        public void Configure(EntityTypeBuilder<PromoCode> builder)
        {
            builder.HasKey(p => p.Code);

            builder.Property(p => p.Code)
                .HasMaxLength(50)
                .ValueGeneratedNever();

            builder.Property(p => p.Value)
                .HasPrecision(18, 2);

            builder.Property(p => p.MinimumOrder)
                .HasPrecision(18, 2);

            builder.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_PromoCode_Dates",
                    $"[{nameof(PromoCode.ExpirationDate)}] >= [{nameof(PromoCode.StartDate)}]"
                );

                t.HasCheckConstraint(
                    "CK_PromoCode_UsedCount",
                    $"[{nameof(PromoCode.UsedCount)}] >= 0"
                );
            });
        }
    }
}