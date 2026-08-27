using Application.Features.Account.Services;
using MediatR;

namespace Application.Features.Account.Commands.RevokeToken;

internal class RevokeAccountTokenCommandHandler(AccountTokenService tokenService) : IRequestHandler<RevokeAccountTokenCommand>
{
    public Task Handle(RevokeAccountTokenCommand request, CancellationToken cancellationToken)
        => tokenService.RevokeAsync(request.RefreshToken, cancellationToken);
}

