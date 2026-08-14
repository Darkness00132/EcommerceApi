using Domain.Entities.OrdersAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.OrdersAggregate;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(x => x.Subtotal)
            .HasPrecision(18, 2);

        builder.Property(x => x.ShippingFee)
            .HasPrecision(18, 2);

        builder.Property(x => x.ItemsDiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.PromoDiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.Total)
            .HasPrecision(18, 2);

        builder.OwnsOne(x => x.ShippingAddress, address =>
        {
            address.Property(x => x.Street)
                .HasMaxLength(250);

            address.Property(x => x.City)
                .HasMaxLength(100);

            address.Property(x => x.Phone)
                .HasMaxLength(20);

            address.Property(x => x.Notes)
                .HasMaxLength(250);
        });
    }
}