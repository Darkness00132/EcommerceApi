using Domain.Entities.Identity;
using Domain.Exceptions;

namespace Domain.Test.Identity;

public class RefreshTokenTests
{
    private static readonly Guid DefaultId = Guid.NewGuid();
    private static readonly Guid DefaultUserId = Guid.NewGuid();
    private const string DefaultToken = "sample-refresh-token-12345";

    [Fact]
    public void Constructor_WithValidData_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var expiresAt = DateTime.UtcNow.AddDays(7);

        // Act
        var token = new RefreshToken(DefaultId, DefaultUserId, "  " + DefaultToken + "  ", expiresAt);

        // Assert
        Assert.Equal(DefaultId, token.Id);
        Assert.Equal(DefaultUserId, token.UserId);
        Assert.Equal(DefaultToken, token.Token);
        Assert.Equal(expiresAt, token.ExpiresAt);
        Assert.True(token.IsActive);
        Assert.Null(token.RevokedAt);
    }

    [Fact]
    public void Constructor_WithEmptyId_ShouldThrowDomainException()
    {
        // Act & Assert
        Assert.Throws<DomainException>(() =>
            new RefreshToken(Guid.Empty, DefaultUserId, DefaultToken, DateTime.UtcNow.AddDays(1)));
    }

    [Fact]
    public void Constructor_WithEmptyUserId_ShouldThrowDomainException()
    {
        // Act & Assert
        Assert.Throws<DomainException>(() =>
            new RefreshToken(DefaultId, Guid.Empty, DefaultToken, DateTime.UtcNow.AddDays(1)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidTokenValue_ShouldThrowDomainException(string? invalidToken)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() =>
            new RefreshToken(DefaultId, DefaultUserId, invalidToken!, DateTime.UtcNow.AddDays(1)));
    }

    [Fact]
    public void Constructor_WithPastExpiration_ShouldThrowDomainException()
    {
        // Act & Assert
        Assert.Throws<DomainException>(() =>
            new RefreshToken(DefaultId, DefaultUserId, DefaultToken, DateTime.UtcNow.AddMinutes(-10)));
    }

    [Fact]
    public void Revoke_WhenActive_ShouldSetRevokedAtAndDeactivate()
    {
        // Arrange
        var token = new RefreshToken(DefaultId, DefaultUserId, DefaultToken, DateTime.UtcNow.AddDays(1));

        // Act
        token.Revoke();

        // Assert
        Assert.NotNull(token.RevokedAt);
        Assert.False(token.IsActive);
    }

    [Fact]
    public void Revoke_WhenAlreadyRevoked_ShouldNotModifyRevokedAtTimestamp()
    {
        // Arrange
        var token = new RefreshToken(DefaultId, DefaultUserId, DefaultToken, DateTime.UtcNow.AddDays(1));
        token.Revoke();
        var initialRevokedAt = token.RevokedAt;

        // Act
        token.Revoke();

        // Assert
        Assert.Equal(initialRevokedAt, token.RevokedAt);
    }
}
