using MediatR;

namespace Application.Features.Product.PatchProductStatus
{
    public record PatchProductStatusCommand(int id) : IRequest;
}
