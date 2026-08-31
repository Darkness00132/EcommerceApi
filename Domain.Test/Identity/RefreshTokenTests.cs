using Domain.Entities.Identity;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Test.Identity;

public class RefreshTokenTests
{
    private static readonly Guid DefaultId = Guid.NewGuid();
    private const string DefaultToken = "sample-refresh-token-12345";

    [Fact]
    public void Constructor_WithValidData_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var user = CreateTestUser("john.doe@example.com");
        var expiresAt = DateTime.UtcNow.AddDays(7);

        // Act
        var token = new RefreshToken(DefaultId, user, "  " + DefaultToken + "  ", expiresAt);

        // Assert
        Assert.Equal(DefaultId, token.Id);
        Assert.Equal(user.Id, token.UserId);
        Assert.Same(user, token.User);
        Assert.Equal(DefaultToken, token.Token);
        Assert.Equal(expiresAt, token.ExpiresAt);
        Assert.True(token.IsActive);
        Assert.Null(token.RevokedAt);
    }

    [Fact]
    public void Constructor_WithEmptyId_ShouldThrowDomainException()
    {
        // Arrange
        var user = CreateTestUser("john.doe@example.com");

        // Act & Assert
        Assert.Throws<DomainException>(() =>
            new RefreshToken(Guid.Empty, user, DefaultToken, DateTime.UtcNow.AddDays(1)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidTokenValue_ShouldThrowDomainException(string? invalidToken)
    {
        // Arrange
        var user = CreateTestUser("john.doe@example.com");

        // Act & Assert
        Assert.Throws<DomainException>(() =>
            new RefreshToken(DefaultId, user, invalidToken!, DateTime.UtcNow.AddDays(1)));
    }

    [Fact]
    public void Constructor_WithPastExpiration_ShouldThrowDomainException()
    {
        // Arrange
        var user = CreateTestUser("john.doe@example.com");

        // Act & Assert
        Assert.Throws<DomainException>(() =>
            new RefreshToken(DefaultId, user, DefaultToken, DateTime.UtcNow.AddMinutes(-10)));
    }

    [Fact]
    public void Revoke_WhenActive_ShouldSetRevokedAtAndDeactivate()
    {
        // Arrange
        var user = CreateTestUser("john.doe@example.com");
        var token = new RefreshToken(DefaultId, user, DefaultToken, DateTime.UtcNow.AddDays(1));

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
        var user = CreateTestUser("john.doe@example.com");
        var token = new RefreshToken(DefaultId, user, DefaultToken, DateTime.UtcNow.AddDays(1));
        token.Revoke();
        var initialRevokedAt = token.RevokedAt;

        // Act
        token.Revoke();

        // Assert
        Assert.Equal(initialRevokedAt, token.RevokedAt);
    }

    private static AppUser CreateTestUser(string email)
    {
        return new AppUser(new FullName("John", "Doe"), email);
    }
}
