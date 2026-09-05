using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Abstractions.Services;
using Application.Settings;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

internal class JwtTokenService : IJwtTokenService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly JwtSettings _settings;

    public JwtTokenService(UserManager<AppUser> userManager, IOptions<JwtSettings> jwtOptions)
    {
        _userManager = userManager;
        _settings = jwtOptions.Value;
    }

    public async Task<(string Token, DateTime ExpiresAt)> GenerateAsync(
        AppUser user,
        CancellationToken cancellationToken = default)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(
            _settings.AccessTokenExpirationInMinutes);

        var claims = await GetClaimsAsync(user);

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key)),
            SecurityAlgorithms.HmacSha256);

        var securityToken = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: signingCredentials);

        var token = new JwtSecurityTokenHandler()
            .WriteToken(securityToken);

        return (token, expiresAt);
    }

    private async Task<List<Claim>> GetClaimsAsync(AppUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(
            roles.Select(role => new Claim(ClaimTypes.Role, role))
            );

        return claims;
    }
}
