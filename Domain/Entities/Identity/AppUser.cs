using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Exceptions;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Identity;

public sealed class AppUser : IdentityUser<Guid>, IEntity
{
    private readonly List<RefreshToken> _refreshTokens = new();

    public FullName FullName { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }

    [NotMapped]
    public string DisplayName => FullName?.ToString() ?? string.Empty;

    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    private AppUser() { } // Required for EF Core & ASP.NET Core Identity

    public AppUser(FullName fullName, string email)
    {
        if (fullName is null)
            throw new DomainException("Full name is required.");

        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email is required.");

        var trimmedEmail = email.Trim();

        Id = Guid.NewGuid();
        FullName = fullName;
        Email = trimmedEmail;
        UserName = trimmedEmail;
        CreatedAt = DateTime.UtcNow;
    }

    public void ChangeName(FullName fullName)
    {
        FullName = fullName ?? throw new DomainException("Full name is required.");
    }

    public RefreshToken AddRefreshToken(string token, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new DomainException("Refresh token is required.");

        if (expiresAt <= DateTime.UtcNow)
            throw new DomainException("Refresh token expiration must be in the future.");

        var refreshToken = new RefreshToken(Guid.NewGuid(), this, token.Trim(), expiresAt);
        _refreshTokens.Add(refreshToken);

        return refreshToken;
    }

    public void RevokeRefreshToken(RefreshToken refreshToken)
    {
        if (refreshToken is null)
            throw new DomainException("Refresh token is required.");

        refreshToken.Revoke();

        _refreshTokens.Remove(refreshToken);
    }
}
