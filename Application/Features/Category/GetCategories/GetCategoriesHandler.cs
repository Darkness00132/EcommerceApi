using Application.Common.Pagination;
using Application.Features.Category.Dtos;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Features.Category.GetCategories
{
    public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, PaginationResult<CategoryResponse>>
    {
        private readonly ICategoryRepository _categoryRepository;

        public GetCategoriesHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<PaginationResult<CategoryResponse>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await _categoryRepository.GetAllCategories(request.Pagination,cancellationToken);
        }
    }
}
