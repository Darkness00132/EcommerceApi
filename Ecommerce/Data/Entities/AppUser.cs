using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Data.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Cart? Cart { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
        public ICollection<Order> Orders { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = [];
    }
}
