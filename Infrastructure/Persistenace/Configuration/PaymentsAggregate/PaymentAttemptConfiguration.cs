using Domain.Entities.PaymentsAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.PaymentsAggregate;

public sealed class PaymentAttemptConfiguration : IEntityTypeConfiguration<PaymentAttempt>
{
    public void Configure(EntityTypeBuilder<PaymentAttempt> builder)
    {
        builder.Property(x => x.Amount)
            .HasPrecision(18, 2);

        builder.Property(x => x.TransactionId)
            .HasMaxLength(100);

        builder.Property(x => x.GatewayResponse)
            .HasMaxLength(2000);
    }
}