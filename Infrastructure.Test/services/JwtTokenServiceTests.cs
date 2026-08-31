using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Settings;
using Domain.Constants;
using Domain.Entities.Identity;
using Domain.ValueObjects;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;

namespace Infrastructure.Test.services;

public class JwtTokenServiceTests
{
    private readonly Mock<UserManager<AppUser>> _userManager;
    private readonly JwtTokenService _service;

    public JwtTokenServiceTests()
    {
        var userStore = new Mock<IUserStore<AppUser>>();

        _userManager = new Mock<UserManager<AppUser>>(
            userStore.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);

        var jwtOptions = Options.Create(new JwtSettings {
            Key = "this-is-a-test-key-that-is-long-enough",
            Issuer = "test-issuer",
            Audience = "test-audience",
            AccessTokenExpirationInMinutes = 60
        });

        _service = new JwtTokenService(
            _userManager.Object,
            jwtOptions);
    }

    [Theory]
    [MemberData(nameof(RoleCases))]
    public async Task GenerateAsync_ShouldIncludeUserRoles(
        string[] roles)
    {
        // Arrange
        var user = new AppUser(
            new FullName("John", "Doe"),
            "test@example.com");

        _userManager
            .Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(roles);

        // Act
        var result = await _service.GenerateAsync(user);

        var token = new JwtSecurityTokenHandler()
            .ReadJwtToken(result.Token);

        var actualRoles = token.Claims
            .Where(x => x.Type == ClaimTypes.Role)
            .Select(x => x.Value);

        // Assert
        Assert.Equal(
            roles.OrderBy(x => x),
            actualRoles.OrderBy(x => x));

        _userManager.Verify(
            x => x.GetRolesAsync(user),
            Times.Once);
    }

    public static TheoryData<string[]> RoleCases => new()
    {
        { Array.Empty<string>() },
        { new[] { AppRoles.Customer } },
        { new[] { AppRoles.Admin } },
        {
            new[]
            {
                AppRoles.Customer,
                AppRoles.SalesManager
            }
        },
        {
            new[]
            {
                AppRoles.Admin,
                AppRoles.CatalogManager
            }
        },
        {
            new[]
            {
                AppRoles.SuperAdmin,
                AppRoles.Admin,
                AppRoles.InventoryManager
            }
        }
    };
}
