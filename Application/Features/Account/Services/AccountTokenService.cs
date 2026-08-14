using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.Features.Account.Dto;
using Application.Settings;
using Domain.Entities.Identity;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Application.Features.Account.Services;

internal class AccountTokenService(IRepository<RefreshToken> refreshTokens, IUnitOfWork unitOfWork, IJwtTokenService jwtTokenService, IOptions<JwtSettings> jwtOptions)
{
    public async Task<AccountTokenDto> CreateAsync(AppUser user, CancellationToken cancellationToken)
    {
        var (accessToken, accessExpires) = await jwtTokenService.GenerateAsync(user, cancellationToken);

        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var refreshExpires = DateTime.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpirationInDays);

        user.AddRefreshToken(refreshToken, refreshExpires);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new AccountTokenDto(accessToken, refreshToken, accessExpires, refreshExpires);
    }

    public async Task<AccountTokenDto> RefreshAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var token = await refreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken, cancellationToken, x => x.User);
        if (token is null || !token.IsActive)
            throw new UnauthorizedAccessException("The refresh token is invalid or expired.");

        token.Revoke();
        return await CreateAsync(token.User, cancellationToken);
    }

    public async Task RevokeAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var token = await refreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken, cancellationToken);
        if (token is null || !token.IsActive) return;

        token.Revoke();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

