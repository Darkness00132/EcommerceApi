using Domain.Entities.ProcurementAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.ProcurementAggregate;

public sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(200);

        builder.Property(x => x.ContactName)
            .HasMaxLength(150);

        builder.Property(x => x.Email)
            .HasMaxLength(256);

        builder.Property(x => x.Phone)
            .HasMaxLength(20);

        builder.Property(x => x.Address)
            .HasMaxLength(250);

        builder.Property(x => x.City)
            .HasMaxLength(100);

        builder.Property(x => x.TaxNumber)
            .HasMaxLength(100);
    }
}