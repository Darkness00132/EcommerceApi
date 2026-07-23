namespace Ecommerce.Api.Contracts.Product
{
    public record CreateProductRequest(
        string NameEn,
        string NameAr,
        string SKU,
        string? DescriptionEn,
        string? DescriptionAr,
        decimal Price,
        int Stock,
        string Brand,
        bool IsActive,
        int CategoryId,
        int? DiscountId,
        List<IFormFile> Images);
}
