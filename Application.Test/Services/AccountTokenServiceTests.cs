using System.Linq.Expressions;
using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.Features.Account.Services;
using Application.Settings;
using Domain.Entities.Identity;
using Domain.ValueObjects;
using Microsoft.Extensions.Options;
using Moq;

namespace Application.Test.Services;

public class AccountTokenServiceTests
{
    private readonly Mock<IRepository<RefreshToken>> _refreshTokenRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly Mock<IOptions<JwtSettings>> _jwtOptionsMock;
    private readonly AccountTokenService _sut;

    private readonly JwtSettings _jwtSettings;

    public AccountTokenServiceTests()
    {
        _refreshTokenRepoMock = new Mock<IRepository<RefreshToken>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _jwtOptionsMock = new Mock<IOptions<JwtSettings>>();

        _jwtSettings = new JwtSettings { RefreshTokenExpirationInDays = 7 };
        _jwtOptionsMock.Setup(o => o.Value).Returns(_jwtSettings);

        _sut = new AccountTokenService(
            _refreshTokenRepoMock.Object,
            _unitOfWorkMock.Object,
            _jwtTokenServiceMock.Object,
            _jwtOptionsMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldGenerateTokensAddRefreshTokenToUserAndSave()
    {
        // Arrange
        var user = CreateTestUser("john.doe@example.com");
        var expectedAccessToken = "access-token-123";
        var expectedAccessExpires = DateTime.UtcNow.AddMinutes(15);
        var cancellationToken = CancellationToken.None;

        _jwtTokenServiceMock
            .Setup(j => j.GenerateAsync(user, cancellationToken))
            .ReturnsAsync((expectedAccessToken, expectedAccessExpires));

        // Act
        var result = await _sut.CreateAsync(user, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedAccessToken, result.AccessToken);
        Assert.Equal(expectedAccessExpires, result.AccessTokenExpiresAt);
        Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
        Assert.True(result.RefreshTokenExpiresAt > DateTime.UtcNow);
        Assert.Single(user.RefreshTokens);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task RefreshAsync_ShouldThrowUnauthorizedAccessException_WhenTokenNotFoundOrInactive()
    {
        // Arrange
        var invalidRefreshToken = "non-existent-token";
        var cancellationToken = CancellationToken.None;

        _refreshTokenRepoMock
            .Setup(r => r.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<RefreshToken, bool>>>(),
                cancellationToken,
                It.IsAny<Expression<Func<RefreshToken, object>>>()))
            .ReturnsAsync((RefreshToken?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _sut.RefreshAsync(invalidRefreshToken, cancellationToken));

        Assert.Equal("The refresh token is invalid or expired.", exception.Message);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RefreshAsync_ShouldRevokeOldTokenAndReturnNewTokenDto_WhenTokenIsValid()
    {
        // Arrange
        var user = CreateTestUser("jane.doe@example.com");
        var activeTokenString = "valid-active-refresh-token";

        // Generate token via domain aggregate root
        var refreshToken = user.AddRefreshToken(activeTokenString, DateTime.UtcNow.AddDays(1));

        var newAccessToken = "new-access-token-456";
        var newAccessExpires = DateTime.UtcNow.AddMinutes(15);
        var cancellationToken = CancellationToken.None;

        _refreshTokenRepoMock
            .Setup(r => r.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<RefreshToken, bool>>>(),
                cancellationToken,
                It.IsAny<Expression<Func<RefreshToken, object>>>()))
            .ReturnsAsync(refreshToken);

        _jwtTokenServiceMock
            .Setup(j => j.GenerateAsync(user, cancellationToken))
            .ReturnsAsync((newAccessToken, newAccessExpires));

        // Act
        var result = await _sut.RefreshAsync(activeTokenString, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newAccessToken, result.AccessToken);
        Assert.False(refreshToken.IsActive);
        Assert.NotNull(refreshToken.RevokedAt);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task RevokeAsync_ShouldDoNothing_WhenTokenNotFoundOrInactive()
    {
        // Arrange
        var refreshToken = "invalid-token";
        var cancellationToken = CancellationToken.None;

        _refreshTokenRepoMock
            .Setup(r => r.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<RefreshToken, bool>>>(),
                cancellationToken))
            .ReturnsAsync((RefreshToken?)null);

        // Act
        await _sut.RevokeAsync(refreshToken, cancellationToken);

        // Assert
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RevokeAsync_ShouldRevokeTokenAndSaveChanges_WhenTokenIsActive()
    {
        // Arrange
        var user = CreateTestUser("john.doe@example.com");
        var tokenString = "active-token-to-revoke";
        var refreshToken = user.AddRefreshToken(tokenString, DateTime.UtcNow.AddDays(1));

        var cancellationToken = CancellationToken.None;

        _refreshTokenRepoMock
            .Setup(r => r.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<RefreshToken, bool>>>(),
                cancellationToken))
            .ReturnsAsync(refreshToken);

        // Act
        await _sut.RevokeAsync(tokenString, cancellationToken);

        // Assert
        Assert.False(refreshToken.IsActive);
        Assert.NotNull(refreshToken.RevokedAt);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(cancellationToken), Times.Once);
    }

    private static AppUser CreateTestUser(string email)
    {
        return new AppUser(new FullName("John", "Doe"), email);
    }
}
