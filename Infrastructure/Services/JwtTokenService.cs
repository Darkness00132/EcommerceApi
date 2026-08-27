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

internal class JwtTokenService(
    UserManager<AppUser> userManager,
    IOptions<JwtSettings> jwtOptions)
    : IJwtTokenService
{
    public async Task<(string Token, DateTime ExpiresAt)> GenerateAsync(
        AppUser user,
        CancellationToken cancellationToken = default)
    {
        var settings = jwtOptions.Value;

        var expiresAt = DateTime.UtcNow.AddMinutes(
            settings.AccessTokenExpirationInMinutes);

        var claims = await GetClaimsAsync(user);

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
            SecurityAlgorithms.HmacSha256);

        var securityToken = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: signingCredentials);

        var token = new JwtSecurityTokenHandler()
            .WriteToken(securityToken);

        return (token, expiresAt);
    }

    private async Task<List<Claim>> GetClaimsAsync(AppUser user)
    {
        var roles = await userManager.GetRolesAsync(user);

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
