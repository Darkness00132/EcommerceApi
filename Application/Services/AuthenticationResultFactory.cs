using Application.Features.Auth.Dtos;
using Application.Interfaces.Services;
using Application.Settings;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Application.Services
{
    public class AuthenticationResultFactory
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenService _refreshTokenService;

        public AuthenticationResultFactory(UserManager<AppUser> userManager, IOptions<JwtSettings> jwtSettings, IJwtService jwtService, IRefreshTokenService refreshTokenService)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<AuthResponse> GenerateAuthResponse(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var refreshToken = await _refreshTokenService.CreateAsync(user);

            return new AuthResponse
            {
                AccessToken = _jwtService.GenerateToken(user, roles),
                RefreshToken = refreshToken.Token,
                AccessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes)
            };
        }
    }
}
