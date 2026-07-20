using Domain.Entities;

namespace Application.Interfaces.Services
{
    public interface IRefreshTokenService
    {
        public Task<RefreshToken> CreateAsync(AppUser user);

        public Task<RefreshToken?> GetValidAsync(string token);

        public Task RevokeAsync(string token);
    }
}
