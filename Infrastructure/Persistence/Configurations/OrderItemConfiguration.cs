using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(oi => new { oi.OrderId, oi.ProductId });

            builder.Property(oi => oi.UnitPrice)
                .HasPrecision(18, 2);

            builder.Property(oi => oi.DiscountAmount)
                .HasPrecision(18, 2);

            builder.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_OrderItem_Quantity",
                    $"[{nameof(OrderItem.Quantity)}] > 0"
                );

                t.HasCheckConstraint(
                    "CK_OrderItem_UnitPrice",
                    $"[{nameof(OrderItem.UnitPrice)}] >= 0"
                );

                t.HasCheckConstraint(
                    "CK_OrderItem_DiscountAmount",
                    $"[{nameof(OrderItem.DiscountAmount)}] >= 0"
                );
            });
        }
    }
}
