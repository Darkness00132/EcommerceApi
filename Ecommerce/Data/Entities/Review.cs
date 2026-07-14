using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Data.Entities
{
    public class Review
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }
        public AppUser User { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        [Range(1,5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
