using Domain.Entities.PaymentsAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.PaymentsAggregate;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasIndex(x => x.OrderId)
            .IsUnique();

        builder.Navigation(x => x.Attempts)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
