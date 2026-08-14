using Domain.Entities.ProcurementAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.ProcurementAggregate;

public sealed class GoodsReceiptItemConfiguration : IEntityTypeConfiguration<GoodsReceiptItem>
{
    public void Configure(EntityTypeBuilder<GoodsReceiptItem> builder)
    {
        builder.HasIndex(x => new { x.GoodsReceiptId, x.ProductId })
            .IsUnique();
    }
}