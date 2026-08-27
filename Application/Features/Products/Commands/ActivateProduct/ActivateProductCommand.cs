using MediatR;

namespace Application.Features.Products.Commands.ActivateProduct;

public sealed record ActivateProductCommand(Guid ProductId) : IRequest;
