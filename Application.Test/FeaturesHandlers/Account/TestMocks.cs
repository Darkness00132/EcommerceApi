using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.Features.Account.Services;
using Application.Settings;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;

namespace Application.Test.FeaturesHandlers.Account;

internal static class TestMocks
{
    public static Mock<UserManager<AppUser>> CreateMockUserManager()
    {
        var storeMock = new Mock<IUserStore<AppUser>>();

        return new Mock<UserManager<AppUser>>(
            storeMock.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);
    }
    public static AccountTokenService CreateMockAccountTokenService()
    {
        var refreshTokenRepo = new Mock<IRepository<RefreshToken>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var jwtTokenService = new Mock<IJwtTokenService>();

        var jwtOptions = Options.Create(new JwtSettings {
            Key = "ThisIsASecretKeyForJwtTokenGeneration12345",
            Audience = "TestAudience",
            Issuer = "TestIssuer",
            AccessTokenExpirationInMinutes = 15,
            RefreshTokenExpirationInDays = 7
        });

        return new AccountTokenService(
            refreshTokenRepo.Object,
            unitOfWork.Object,
            jwtTokenService.Object,
            jwtOptions);
    }
}
