using MediatR;

namespace Application.Features.Products.Commands.ChangeProductPrice;

public sealed record ChangeProductPriceCommand(
    Guid ProductId,
    decimal Price) : IRequest;
