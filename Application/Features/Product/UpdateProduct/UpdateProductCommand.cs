using Application.Common;
using MediatR;

namespace Application.Features.Product.UpdateProduct
{
    public record UpdateProductCommand(int Id,
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
        List<FileDto>? Images) : IRequest;
}
