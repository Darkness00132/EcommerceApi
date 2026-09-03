using System.Security.Cryptography;
using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.Features.Account.Dto;
using Application.Settings;
using Domain.Entities.Identity;
using Microsoft.Extensions.Options;

namespace Application.Features.Account.Services;

internal class AccountTokenService
{
    private readonly IRepository<RefreshToken> _refreshTokens;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IOptions<JwtSettings> _jwtOptions;

    public AccountTokenService(IRepository<RefreshToken> refreshTokens, IUnitOfWork unitOfWork, IJwtTokenService jwtTokenService, IOptions<JwtSettings> jwtOptions)
    {
        _refreshTokens = refreshTokens;
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
        _jwtOptions = jwtOptions;
    }

    public async Task<AccountTokenDto> CreateAsync(AppUser user, CancellationToken cancellationToken)
    {
        var (accessToken, accessExpires) = await _jwtTokenService.GenerateAsync(user, cancellationToken);

        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var refreshExpires = DateTime.UtcNow.AddDays(_jwtOptions.Value.RefreshTokenExpirationInDays);

        var refreshTokenEntity = user.AddRefreshToken(refreshToken, refreshExpires);
        await _refreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AccountTokenDto(accessToken, refreshToken, accessExpires, refreshExpires);
    }

    public async Task<AccountTokenDto> RefreshAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var token = await _refreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken, cancellationToken, x => x.User);
        if (token is null || !token.IsActive)
            throw new UnauthorizedAccessException("The refresh token is invalid or expired.");

        token.User.RevokeRefreshToken(token);
        return await CreateAsync(token.User, cancellationToken);
    }

    public async Task RevokeAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var token = await _refreshTokens.SingleOrDefaultAsync(
            x => x.Token == refreshToken,
            cancellationToken,
            x => x.User);
        if (token is null || !token.IsActive) return;

        token.User.RevokeRefreshToken(token);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

