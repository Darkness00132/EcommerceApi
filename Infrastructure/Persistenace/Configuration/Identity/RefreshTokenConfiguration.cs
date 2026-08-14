using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistenace.Configuration.Identity;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {

        builder.Property(x => x.Token)
            .HasMaxLength(256);

        builder.HasIndex(x => x.Token)
            .IsUnique();

        builder.Property(x => x.ExpiresAt);

        builder.Property(x => x.CreatedAt);

        builder.Property(x => x.RevokedAt);
    }
}