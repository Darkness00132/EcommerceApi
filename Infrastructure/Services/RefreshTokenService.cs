using Application.Interfaces.Services;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Infrastructure.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly AppDbContext _context;

        public RefreshTokenService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<RefreshToken> CreateAsync(AppUser user)
        {
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }


        public async Task<RefreshToken?> GetValidAsync(string token)
        {
            return await _context.RefreshTokens
                .AsNoTracking()
                .Include(x => x.User)
                .FirstOrDefaultAsync(x =>
                    x.Token == token &&
                    x.RevokedAt == null &&
                    x.ExpiresAt > DateTime.UtcNow);
        }

        public async Task RevokeAsync(string token)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == token);

            if (refreshToken == null)
                return;

            refreshToken.RevokedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}