namespace Ecommerce.Api.Contracts.Category
{
    public record CreateCategoryRequest(string NameEn,
         string NameAr,
         string? DescriptionEn,
         string? DescriptionAr,
         IFormFile? Image);
}
