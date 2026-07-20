namespace Application.DTOs.Category
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }

        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }

        public string? ImageUrl { get; set; }
    }
}
