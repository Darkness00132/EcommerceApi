using Application.Common.Files;
using MediatR;

namespace Application.Features.Products.Commands.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid Id,
    string? NameEn,
    string? NameAr,
    string? SKU,
    string? DescriptionEn,
    string? DescriptionAr,
    decimal? Price,
    Guid? CategoryId,
    Guid? BrandId,
    List<string>? DeletedImages,
    List<FileDto>? NewImages) : IRequest;
