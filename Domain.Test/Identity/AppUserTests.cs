using Domain.Entities.Identity;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Test.Identity;

public class AppUserTests
{
    private static readonly FullName DefaultFullName = new("John", "Doe");
    private const string DefaultEmail = "john.doe@example.com";

    [Fact]
    public void Constructor_WithValidData_ShouldInitializePropertiesCorrectly()
    {
        // Act
        var user = new AppUser(DefaultFullName, DefaultEmail);

        // Assert
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(DefaultFullName, user.FullName);
        Assert.Equal(DefaultEmail, user.Email);
        Assert.Equal(DefaultEmail, user.UserName);
        Assert.Equal("John Doe", user.DisplayName);
        Assert.Empty(user.RefreshTokens);
    }

    [Fact]
    public void Constructor_WithNullFullName_ShouldThrowDomainException()
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => new AppUser(null!, DefaultEmail));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidEmail_ShouldThrowDomainException(string? invalidEmail)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => new AppUser(DefaultFullName, invalidEmail!));
    }

    [Fact]
    public void ChangeName_WithValidFullName_ShouldUpdateFullName()
    {
        // Arrange
        var user = new AppUser(DefaultFullName, DefaultEmail);
        var newName = new FullName("Jane", "Smith");

        // Act
        user.ChangeName(newName);

        // Assert
        Assert.Equal(newName, user.FullName);
        Assert.Equal("Jane Smith", user.DisplayName);
    }

    [Fact]
    public void AddRefreshToken_WithValidToken_ShouldAddToCollection()
    {
        // Arrange
        var user = new AppUser(DefaultFullName, DefaultEmail);
        var expiresAt = DateTime.UtcNow.AddDays(7);

        // Act
        user.AddRefreshToken("sample-refresh-token", expiresAt);

        // Assert
        Assert.Single(user.RefreshTokens);
    }

    [Fact]
    public void AddRefreshToken_WithExpiredDate_ShouldThrowDomainException()
    {
        // Arrange
        var user = new AppUser(DefaultFullName, DefaultEmail);
        var expiredAt = DateTime.UtcNow.AddMinutes(-10);

        // Act & Assert
        Assert.Throws<DomainException>(() => user.AddRefreshToken("sample-token", expiredAt));
    }

    [Fact]
    public void RevokeRefreshToken_WithExistingToken_ShouldRevokeToken()
    {
        // Arrange
        var user = new AppUser(DefaultFullName, DefaultEmail);
        var token = "token-to-revoke";
        user.AddRefreshToken(token, DateTime.UtcNow.AddDays(1));

        // Act
        user.RevokeRefreshToken(token);

        // Assert
        var refreshToken = Assert.Single(user.RefreshTokens);
        Assert.NotNull(refreshToken.RevokedAt);
        Assert.False(refreshToken.IsActive);
    }

    [Fact]
    public void RevokeRefreshToken_WithNonExistingToken_ShouldThrowDomainException()
    {
        // Arrange
        var user = new AppUser(DefaultFullName, DefaultEmail);

        // Act & Assert
        Assert.Throws<DomainException>(() => user.RevokeRefreshToken("non-existing-token"));
    }
}
