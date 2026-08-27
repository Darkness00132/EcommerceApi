using Application.Common.Files;
using MediatR;

namespace Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string NameEn,
    string NameAr,
    string DescriptionEn,
    string DescriptionAr,
    string SKU,
    decimal Price,
    bool IsVisible,
    Guid CategoryId,
    Guid BrandId,
    Guid? DiscountId,
    List<FileDto> Images) : IRequest<Guid>;
