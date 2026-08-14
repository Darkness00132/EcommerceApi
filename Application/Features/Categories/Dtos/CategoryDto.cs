namespace Application.Features.Categories.Dtos;

public sealed record CategoryDto(
    Guid Id,
    string NameEn,
    string NameAr,
    string? DescriptionEn,
    string? DescriptionAr,
    string ImageKey);
