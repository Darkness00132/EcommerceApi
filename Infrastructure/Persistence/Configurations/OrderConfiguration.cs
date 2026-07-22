using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(o => o.Subtotal)
                .HasPrecision(18, 2);
            builder.Property(o => o.DiscountAmount)
                .HasPrecision(18, 2);
            builder.Property(o => o.Total)
                .HasPrecision(18, 2);

            builder.Property(o => o.ShippingAddress)
                .HasMaxLength(250)
                .IsRequired();
            builder.Property(o => o.ShippingCity)
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(o => o.ShippingPhone)
                .HasMaxLength(20)
                .IsRequired();
        }
    }
}