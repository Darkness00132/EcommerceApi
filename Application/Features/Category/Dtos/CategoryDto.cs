namespace Application.Features.Category.Dtos
{
    public record CategoryDto(int Id,
        string NameEn,
        string NameAr,
        string? DescriptionEn,
        string? DescriptionAr)
    {
        public string? ImageUrl { get; set; }
    }
}
