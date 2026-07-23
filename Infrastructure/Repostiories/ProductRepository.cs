using Application.Common.Filters;
using Application.Common.Pagination;
using Application.Features.Product.Dto;
using Application.Interfaces.Repositories;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repostiories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfigurationProvider _mapperConfig;

        public ProductRepository(AppDbContext context, IConfigurationProvider mapperConfig)
        {
            _context = context;
            _mapperConfig = mapperConfig;
        }

        public async Task CreateAsync(Product product, CancellationToken ct = default)
        {
            await _context.Products.AddAsync(product, ct);
        }

        public void Delete(Product product)
        {
            _context.Remove(product);
        }

        public async Task<DetailedProductResponse?> GetDetailedProductById(int id, CancellationToken ct = default)
        {
            return await _context.Products
                    .Where(p => p.Id == id)
                    .ProjectTo<DetailedProductResponse>(_mapperConfig)
                    .FirstOrDefaultAsync(ct);
        }

        public async Task<PaginationResult<ProductItemResponse>> GetFilteredProducts(ProductFilters filters, PaginationRequest pagination, CancellationToken ct = default)
        {
            var query = _context.Products
                .AsNoTracking()
                .Where(p => p.IsActive)
                .ApplySearch(filters.SearchTerm)
                .ApplyCategory(filters.CategoryId)
                .ApplyBrand(filters.Brand)
                .ApplyStock(filters.InStock)
                .ApplyPriceRange(filters.MinPrice, filters.MaxPrice)
                .ApplySorting(filters.SortBy);

            var counts = await query.CountAsync(ct);
            var products = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ProjectTo<ProductItemResponse>(_mapperConfig)
                .ToListAsync(ct);

            return new PaginationResult<ProductItemResponse>(products,
                pagination.PageSize,
                pagination.PageNumber,
                counts);
        }

        public async Task<Product?> GetProductByIdWithTracking(int id, CancellationToken ct = default)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id==id,ct);
        }

        public async Task<PaginationResult<ProductItemResponse>> GetProductsWithinCategory(int id, PaginationRequest paginationRequest, CancellationToken ct = default)
        {
            var query = _context.Products.AsNoTracking();

            var products = await query
                .Skip((paginationRequest.PageNumber -1)*paginationRequest.PageSize)
                .Take(paginationRequest.PageSize)
                .ProjectTo<ProductItemResponse>(_mapperConfig)
                .ToListAsync(ct);

            var productsCount = await query.CountAsync(ct);

            return new PaginationResult<ProductItemResponse>(products,
                paginationRequest.PageNumber,
                paginationRequest.PageSize,
                productsCount);
        }
    }
}
