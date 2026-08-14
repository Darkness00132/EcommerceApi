using Domain.Entities.Identity;

namespace Application.Abstractions.Services;

public interface IJwtTokenService
{
    Task<(string Token, DateTime ExpiresAt)> GenerateAsync(AppUser user, CancellationToken cancellationToken = default);
}
