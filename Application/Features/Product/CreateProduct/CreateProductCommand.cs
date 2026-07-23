using Application.Common;
using MediatR;

namespace Application.Features.Product.CreateProduct
{
    public record CreateProductCommand(
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
        List<FileDto> Images) : IRequest
    {
    }
}
