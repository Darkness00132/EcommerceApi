using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
    {
        public void Configure(EntityTypeBuilder<Discount> builder)
        {
            builder.Property(d => d.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(d => d.Value)
                .HasPrecision(18, 2);

            builder.ToTable(t => t.HasCheckConstraint(
                "CK_Discount_DateRange",
                $"[{nameof(Discount.EndDate)}] >= [{nameof(Discount.StartDate)}]"
            ));
        }
    }
}