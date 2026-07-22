using Application.Common.Pagination;
using Application.Features.Category.Dtos;
using Application.Interfaces.Repositories;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repostiories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfigurationProvider _mapperConfig;

        public CategoryRepository(AppDbContext context, IConfigurationProvider mapperConfig)
        {
            _context = context;
            _mapperConfig = mapperConfig;
        }

        public async Task CreateAsync(Category category, CancellationToken ct = default)
        {
            await _context.Categories.AddAsync(category);
        }

        public void Delete(Category category)
        {
            _context.Categories.Remove(category);
        }

        public async Task<PaginationResult<CategoryResponse>> GetAllCategories(PaginationRequest paginationRequest, CancellationToken ct = default)
        {
            var query = _context.Categories.AsNoTracking();
            var catergories = await query
                .Skip((paginationRequest.PageNumber - 1) * paginationRequest.PageSize)
                .Take(paginationRequest.PageSize)
                .ProjectTo<CategoryResponse>(_mapperConfig)
                .ToListAsync(ct);

            var totalCount = await query.CountAsync(ct);

            return new PaginationResult<CategoryResponse>(
                catergories,
                paginationRequest.PageNumber,
                paginationRequest.PageSize,
                totalCount);
        }

        public async Task<Category?> GetCategoryWithTrackingAsync(int id, CancellationToken ct = default)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);
        }
    }
}
