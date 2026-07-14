namespace Ecommerce.Data.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = null!;
        public string NameAr { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        public ICollection<Product> Products { get; set; } = [];
    }
}
