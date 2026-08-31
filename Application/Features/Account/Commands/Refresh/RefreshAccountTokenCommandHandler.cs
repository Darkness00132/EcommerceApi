using Application.Features.Account.Dto;
using Application.Features.Account.Services;
using MediatR;

namespace Application.Features.Account.Commands.Refresh;

internal class RefreshAccountTokenCommandHandler : IRequestHandler<RefreshAccountTokenCommand, AccountTokenDto>
{
    private readonly AccountTokenService _tokenService;

    public RefreshAccountTokenCommandHandler(AccountTokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public Task<AccountTokenDto> Handle(RefreshAccountTokenCommand request, CancellationToken cancellationToken)
        => _tokenService.RefreshAsync(request.RefreshToken, cancellationToken);
}

