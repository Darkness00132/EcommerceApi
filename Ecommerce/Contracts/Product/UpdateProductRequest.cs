using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Contracts.Product
{
    public record UpdateProductRequest(
        string? NameEn,
        string? NameAr,
        string? SKU,
        string? DescriptionEn,
        string? DescriptionAr,
        decimal? Price,
        string? Brand,
        int? CategoryId,
        int? DiscountId,
        List<string>? DeletedImagesKeys,
        List<IFormFile>? Images);
}
