using MediatR;

namespace Application.Features.Product.DeleteProduct
{
    public record DeleteProductCommand(int id) : IRequest;
}
