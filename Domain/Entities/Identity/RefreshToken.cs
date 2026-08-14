using Domain.Common;
using Domain.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Identity;

public sealed class RefreshToken : Entity
{
    public string Token { get; private set; } = null!;

    public DateTime ExpiresAt { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? RevokedAt { get; private set; }

    public Guid UserId { get; private set; }

    public AppUser User { get; private set; } = null!;

    [NotMapped]
    public bool IsActive => RevokedAt is null && ExpiresAt > DateTime.UtcNow;

    private RefreshToken() { }
    public RefreshToken(
        Guid id,
        Guid userId,
        string token,
        DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new DomainException("Refresh token is required.");

        if (expiresAt <= DateTime.UtcNow)
            throw new DomainException("Refresh token expiration must be in the future.");

        Id = id;
        UserId = userId;
        Token = token;
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