using Application.Common.Pagination;
using Application.Features.Category.Dtos;
using MediatR;

namespace Application.Features.Category.GetCategories
{
    public record GetCategoriesQuery(PaginationRequest Pagination) : IRequest<PaginationResult<CategoryResponse>>;
}
