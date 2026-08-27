using Domain.Entities.InventoryAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.InventoryAggregate;

public sealed class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.ProductId)
            .IsUnique();

        builder.Navigation(x => x.Transactions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
