using MediatR;
using Application.Features.Product.Dto;

namespace Application.Features.Category.GetProductsWithinCategory
{
    public record GetProductsWithinCategoryQuery(int id) : IRequest<IReadOnlyList<ProductItemResponse>>;
}
