using Domain.Entities;

namespace Application.Interfaces.Services
{
    public interface IJwtService
    {
        public string GenerateToken(AppUser user, IList<string> roles);
    }
}
