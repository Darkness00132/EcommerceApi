using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistenace.Configuration.Identity;

public sealed class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.OwnsOne(x => x.FullName, fullName =>
        {
            fullName.Property(x => x.FirstName)
                .HasMaxLength(100);

            fullName.Property(x => x.LastName)
                .HasMaxLength(100);
        });

        builder.Property(x => x.CreatedAt);

        builder.Metadata
            .FindNavigation(nameof(AppUser.RefreshTokens))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}