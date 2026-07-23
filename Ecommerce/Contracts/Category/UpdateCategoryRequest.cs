using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Contracts.Category
{
    public record UpdateCategoryRequest(
        string? NameEn,
         string? NameAr,
         string? DescriptionEn,
         string? DescriptionAr,
         IFormFile Image);
}
