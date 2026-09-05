using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Settings;
using Domain.Entities.Identity;
using FluentAssertions;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;

namespace Infrastructure.Test.Services;

public class JwtTokenServiceTests
{
    private readonly JwtTokenService _sut;
    private readonly Mock<UserManager<AppUser>> _mockUserManager;
    private readonly AppUser _user;

    private readonly JwtSettings _settings = new() {
        Key = "this-is-a-long-enough-secret-key-for-testing",
        Issuer = "test-issuer",
        Audience = "test-audience",
        AccessTokenExpirationInMinutes = 30
    };

    public JwtTokenServiceTests()
    {
        _mockUserManager = CreateUserManager();
        _user = new AppUser(new Domain.ValueObjects.FullName("John", "Doe"), "test@gmail.com");
        _sut = new JwtTokenService(
            _mockUserManager.Object,
            Options.Create(_settings));

        _mockUserManager.Setup(x => x.GetRolesAsync(_user))
            .ReturnsAsync(new List<string>());
    }

    [Fact]
    public async Task Generate_Should_Return_Valid_Token()
    {
        // Arrange & Act
        var result = await _sut.GenerateAsync(_user);

        // Assert
        result.Token.Should().NotBeNullOrWhiteSpace();
        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task Generate_Should_Include_User_Information()
    {
        // Arrange & Act
        var result = await _sut.GenerateAsync(_user);

        // Assert
        var jwt = ReadToken(result.Token);

        jwt.Claims.Should().ContainSingle(
            x => x.Type == JwtRegisteredClaimNames.Sub &&
                 x.Value == _user.Id.ToString());

        jwt.Claims.Should().ContainSingle(
            x => x.Type == JwtRegisteredClaimNames.Email &&
                 x.Value == _user.Email);
    }

    [Fact]
    public async Task Generate_Should_Include_User_Roles()
    {
        // Arrange
        _mockUserManager
            .Setup(x => x.GetRolesAsync(_user))
            .ReturnsAsync(["Admin", "Customer"]);

        // Act
        var result = await _sut.GenerateAsync(_user);

        // Assert
        var jwt = ReadToken(result.Token);

        jwt.Claims
            .Where(x => x.Type == ClaimTypes.Role)
            .Select(x => x.Value)
            .Should()
            .BeEquivalentTo("Admin", "Customer");
    }

    [Fact]
    public async Task Generate_Should_Set_Correct_Expiration()
    {
        // Arrange
        var before = DateTime.UtcNow.AddMinutes(
            _settings.AccessTokenExpirationInMinutes);

        // Act
        var result = await _sut.GenerateAsync(_user);

        // Assert
        result.ExpiresAt.Should()
            .BeCloseTo(before, TimeSpan.FromSeconds(1));
    }

    private JwtSecurityToken ReadToken(string token)
    {
        return new JwtSecurityTokenHandler().ReadJwtToken(token);
    }

    private static Mock<UserManager<AppUser>> CreateUserManager()
    {
        var store = new Mock<IUserStore<AppUser>>();

        return new Mock<UserManager<AppUser>>(
            store.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);
    }
}
