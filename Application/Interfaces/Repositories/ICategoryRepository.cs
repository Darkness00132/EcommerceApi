using Application.Common.Pagination;
using Application.Features.Category.Dtos;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<PaginationResult<CategoryDto>> GetAllCategories(PaginationRequest paginationRequest,CancellationToken ct=default);

        Task<Category?> GetCategoryWithTrackingAsync(int id, CancellationToken ct = default);

        Task CreateAsync(Category category, CancellationToken ct = default);

        void Delete(Category category);
    }
}
