using Application.Common.Pagination;
using Application.Features.Product.Dto;
using Application.Interfaces.Repositories;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Infrastructure.Persistence;
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
