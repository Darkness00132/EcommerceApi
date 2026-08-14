namespace Api.Contracts.Categories;
public sealed record UpdateCategoryRequest(
    string? NameEn,
    string? NameAr,
    string? DescriptionEn,
    string? DescriptionAr,
    IFormFile? NewImage);