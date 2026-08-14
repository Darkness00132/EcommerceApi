using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistenace.Configuration.Identity;

public sealed class AppRoleConfiguration : IEntityTypeConfiguration<AppRole>
{
    public void Configure(EntityTypeBuilder<AppRole> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(100);

        builder.Property(x => x.NormalizedName)
            .HasMaxLength(100);
    }
}