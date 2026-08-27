using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Exceptions;

namespace Domain.Entities.Identity;

public sealed class RefreshToken : Entity
{
    [MaxLength(256)]
    public string Token { get; private set; } = null!;

    public DateTime ExpiresAt { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? RevokedAt { get; private set; }

    public Guid UserId { get; private set; }

    public AppUser User { get; private set; } = null!;

    [NotMapped]
    public bool IsActive => RevokedAt is null && ExpiresAt > DateTime.UtcNow;

    private RefreshToken() { } // Required for EF Core

    public RefreshToken(
        Guid id,
        Guid userId,
        string token,
        DateTime expiresAt)
    {
        if (id == Guid.Empty)
            throw new DomainException("Refresh token ID cannot be empty.");

        if (userId == Guid.Empty)
            throw new DomainException("User ID cannot be empty.");

        if (string.IsNullOrWhiteSpace(token))
            throw new DomainException("Refresh token value is required.");

        if (expiresAt <= DateTime.UtcNow)
            throw new DomainException("Refresh token expiration must be in the future.");

        Id = id;
        UserId = userId;
        Token = token.Trim();
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
    }

    public void Revoke()
    {
        if (RevokedAt is not null)
            return;

        RevokedAt = DateTime.UtcNow;
    }
}
