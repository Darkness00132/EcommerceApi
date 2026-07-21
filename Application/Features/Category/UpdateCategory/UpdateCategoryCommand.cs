using Application.Common;
using MediatR;

namespace Application.Features.Category.UpdateCategory
{
    public record UpdateCategoryCommand(
        int id,
        string? NameEn,
         string? NameAr,
         string? DescriptionEn,
         string? DescriptionAr,
         FileDto? Image) : IRequest;
}
