using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Amount)
                .HasPrecision(18, 2);

            builder.Property(p => p.TransactionId)
                .HasMaxLength(100);

            builder.Property(p => p.GatewayResponse)
                .HasMaxLength(2000);

            builder.HasIndex(p => p.OrderId)
                .IsUnique();

            builder.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_Payment_Amount",
                    $"[{nameof(Payment.Amount)}] >= 0"
                );
            });
        }
    }
}