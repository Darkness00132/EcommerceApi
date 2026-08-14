using Domain.Entities.ProcurementAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.ProcurementAggregate;

public sealed class GoodsReceiptConfiguration : IEntityTypeConfiguration<GoodsReceipt>
{
    public void Configure(EntityTypeBuilder<GoodsReceipt> builder)
    {
        builder.Property(x => x.Number)
            .HasMaxLength(50);

        builder.HasIndex(x => x.Number)
            .IsUnique();

        builder.Property(x => x.DeliveryReference)
            .HasMaxLength(100);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);
    }
}