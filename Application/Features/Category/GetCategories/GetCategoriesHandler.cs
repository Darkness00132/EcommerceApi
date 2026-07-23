using Application.Common.Pagination;
using Application.Features.Category.Dtos;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using MediatR;

namespace Application.Features.Category.GetCategories
{
    public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, PaginationResult<CategoryDto>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IStorageService _storageService;
        public GetCategoriesHandler(ICategoryRepository categoryRepository, IStorageService storageService)
        {
            _categoryRepository = categoryRepository;
            _storageService = storageService;
        }

        public async Task<PaginationResult<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetAllCategories(request.Pagination,cancellationToken);

            var tasks = categories.Items.Select(async category =>
            {
                if (!string.IsNullOrWhiteSpace(category.ImageUrl))
                {
                    // Replaces the temporary key stored in ImageUrl with the actual presigned URL
                    category.ImageUrl = await _storageService.GeneratePresignedUrlAsync(category.ImageUrl);
                }
            });

            await Task.WhenAll(tasks);
            return categories;
        }
    }
}
