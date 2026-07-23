using Application.Common.Filters;
using Application.Common.Pagination;
using Application.Features.Product.Dto;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<PaginationResult<ProductItemResponse>> GetProductsWithinCategory(int id, PaginationRequest paginationRequest, CancellationToken ct = default);

        Task<PaginationResult<ProductItemResponse>> GetFilteredProducts(ProductFilters filters,PaginationRequest pagination,CancellationToken ct = default);

        Task<DetailedProductResponse?> GetDetailedProductById(int id, CancellationToken ct = default);

        Task<Product?> GetProductByIdWithTracking(int id, CancellationToken ct = default);

        Task CreateAsync(Product product,CancellationToken ct=default);

        void Delete(Product product);
    }
}
