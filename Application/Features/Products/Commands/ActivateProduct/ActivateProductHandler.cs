using MediatR;

namespace Application.Features.Products.Commands.ActivateProduct;

internal class ActivateProductHandler : IRequestHandler<ActivateProductCommand>
{
    public Task Handle(ActivateProductCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
