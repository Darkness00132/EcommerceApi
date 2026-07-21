using Application.Common.Pagination;
using Application.Features.Category.Dtos;
using MediatR;

namespace Application.Features.Category.GetCategories
{
    public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, PaginationResult<CategoryResponse>>
    {
        public Task<PaginationResult<CategoryResponse>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
