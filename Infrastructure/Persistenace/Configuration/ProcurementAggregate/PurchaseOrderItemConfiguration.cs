using Domain.Entities.ProcurementAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.ProcurementAggregate;

public sealed class PurchaseOrderItemConfiguration : IEntityTypeConfiguration<PurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
    {
        builder.HasIndex(x => new { x.PurchaseOrderId, x.ProductId })
            .IsUnique();

        builder.Property(x => x.UnitCost)
            .HasPrecision(18, 2);
    }
}