using Application.Common.Pagination;
using Application.Features.Product.Dto;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Features.Category.GetProductsWithinCategory
{
    public class GetProductsWithinCategoryHandler : IRequestHandler<GetProductsWithinCategoryQuery, PaginationResult<ProductItemResponse>>
    {
        private readonly IProductRepository _productRepository;

        public GetProductsWithinCategoryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<PaginationResult<ProductItemResponse>> Handle(GetProductsWithinCategoryQuery request, CancellationToken cancellationToken)
        {
            return await _productRepository.GetProductsWithinCategory(request.id, request.Pagination, cancellationToken);
        }
    }
}
