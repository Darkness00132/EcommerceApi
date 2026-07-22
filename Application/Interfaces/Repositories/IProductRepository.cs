using Application.Common.Pagination;
using Application.Features.Product.Dto;

namespace Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<PaginationResult<ProductItemResponse>> GetProductsWithinCategory(int id, PaginationRequest paginationRequest, CancellationToken ct = default);
    }
}
