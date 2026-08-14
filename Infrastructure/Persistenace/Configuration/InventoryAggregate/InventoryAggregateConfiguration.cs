using Domain.Entities.InventoryAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.InventoryAggregate;

public sealed class InventoryAggregateConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.HasIndex(x => x.ProductId)
            .IsUnique();

        builder.Property(x => x.RowVersion)
            .IsRowVersion();
    }
}