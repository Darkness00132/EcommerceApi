using MediatR;

namespace Application.Features.Account.Commands.RevokeToken;

public sealed record RevokeAccountTokenCommand(string RefreshToken) : IRequest;

