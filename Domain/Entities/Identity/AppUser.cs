using Domain.Exceptions;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Identity;

public sealed class AppUser : IdentityUser<Guid>
{
    public FullName FullName { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }

    [NotMapped]
    public string DisplayName => FullName.ToString();

    private readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    private AppUser() { }
    public AppUser(
        FullName fullName,
        string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email is required.");

        Id = Guid.NewGuid();
        FullName = fullName;

        Email = email.Trim();
        UserName = email.Trim();

        CreatedAt = DateTime.UtcNow;
    }

    public void ChangeName(FullName fullName)
    {
        FullName = fullName;
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
            token: token,
            expiresAt: expiresAt));
    }

    public void RevokeRefreshToken(string token)
    {
        var refreshToken = _refreshTokens.FirstOrDefault(x => x.Token == token);

        if (refreshToken is null)
            throw new DomainException("Refresh token was not found.");

        refreshToken.Revoke();
    }
}