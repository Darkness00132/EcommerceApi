using MediatR;

namespace Application.Features.Product.PatchProductStock
{
    public record PatchProductStockCommand(int id,int stock) : IRequest;
}
