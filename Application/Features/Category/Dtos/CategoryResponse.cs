namespace Application.Features.Category.Dtos
{
    public record CategoryResponse(int Id,
        string NameEn,
        string NameAr,
        string? DescriptionEn,
        string? DescriptionAr,
        string? ImageUrl);
}
