namespace Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public string NameEn { get; set; } = null!;
        public string NameAr { get; set; } = null!;
        public string SKU { get; set; } = null!;
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }

        public decimal Price { get; set; }
        public int Stock { get; set; }

        public string Brand { get; set; } = null!;
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = [];
        public ICollection<OrderItem> OrderItems { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = [];
        public ICollection<Discount> Discounts { get; set; } = [];
    }
}
