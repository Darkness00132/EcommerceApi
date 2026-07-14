namespace Ecommerce.Data.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }

        public string Token { get; set; } = null!;

        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? RevokedAt { get; set; }

        public Guid UserId { get; set; }
        public AppUser User { get; set; } = null!;
    }
}
