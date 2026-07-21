using Application.Common;
using MediatR;

namespace Application.Features.Category.CreateCategory
{
    public record CreateCategoryCommand(
        string NameEn,
         string NameAr,
         string? DescriptionEn,
         string? DescriptionAr,
         FileDto? Image
         ) : IRequest;
}
