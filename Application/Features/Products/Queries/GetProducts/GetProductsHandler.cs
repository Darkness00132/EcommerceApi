using Application.Abstractions.Repositories;
using Application.Common.Pagination;
using Application.Features.Products.Dtos;
using MediatR;

namespace Application.Features.Products.Queries.GetProducts;

internal class GetProductsHandler
    : IRequestHandler<GetProductsQuery, PagedResult<ProductInList>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PagedResult<ProductInList>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        => await _productRepository.ProjectToPagedWithFiltersAsync(request.Pagination, request.Filters);
}
