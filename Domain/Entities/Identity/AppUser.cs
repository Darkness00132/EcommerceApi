using System.ComponentModel.DataAnnotations.Schema;
using Domain.Exceptions;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Identity;

public sealed class AppUser : IdentityUser<Guid>
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

    public void AddRefreshToken(string token, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new DomainException("Refresh token is required.");

        if (expiresAt <= DateTime.UtcNow)
            throw new DomainException("Refresh token expiration must be in the future.");

        _refreshTokens.Add(new RefreshToken(
            id: Guid.NewGuid(),
            userId: Id,
            token: token.Trim(),
            expiresAt: expiresAt));
    }

    public void RevokeRefreshToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new DomainException("Refresh token is required.");

        var trimmedToken = token.Trim();
        var refreshToken = _refreshTokens.FirstOrDefault(x => x.Token == trimmedToken);

        if (refreshToken is null)
            throw new DomainException("Refresh token was not found.");

        refreshToken.Revoke();
    }
}
