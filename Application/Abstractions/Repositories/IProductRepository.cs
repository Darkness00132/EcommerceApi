using Api.Contracts.Common;
using Application.Common.Filters;
using Application.Common.Pagination;
using Application.Features.Products.Dtos;
using Domain.Entities.Catalog;

namespace Application.Abstractions.Repositories;

public interface IProductRepository : IRepository<Product>
{
    public Task<PagedResult<ProductInList>> ProjectToPagedWithFiltersAsync(PaginationRequest? pagination, ProductFilter? filters, CancellationToken cancellationToken = default);
}
