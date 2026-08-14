namespace Ecommerce.Api.Contracts.Categories
{
    public record CreateCategoryRequest(string NameEn,
    string NameAr,
    string? DescriptionEn,
    string? DescriptionAr,
    IFormFile Image);
}
