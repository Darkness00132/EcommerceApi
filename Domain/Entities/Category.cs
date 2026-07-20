namespace Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = null!;
        public string NameAr { get; set; } = null!;

        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }

        public string ImageUrl { get; set; } = null!;

        public ICollection<Product> Products { get; set; } = [];
    }
}
