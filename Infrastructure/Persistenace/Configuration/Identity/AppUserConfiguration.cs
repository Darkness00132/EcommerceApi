using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Identity;

public sealed class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.OwnsOne(x => x.FullName);

        builder.Navigation(x => x.RefreshTokens)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
