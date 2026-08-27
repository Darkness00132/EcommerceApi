using Application.Features.Account.Dto;
using Application.Features.Account.Services;
using MediatR;

namespace Application.Features.Account.Commands.RefreshToken;

internal class RefreshAccountTokenCommandHandler(AccountTokenService tokenService) : IRequestHandler<RefreshAccountTokenCommand, AccountTokenDto>
{
    public Task<AccountTokenDto> Handle(RefreshAccountTokenCommand request, CancellationToken cancellationToken)
        => tokenService.RefreshAsync(request.RefreshToken, cancellationToken);
}

