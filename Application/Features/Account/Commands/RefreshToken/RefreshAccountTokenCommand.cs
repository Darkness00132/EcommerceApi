using Application.Features.Account.Dto;
using MediatR;

namespace Application.Features.Account.Commands.RefreshToken;

public sealed record RefreshAccountTokenCommand(string RefreshToken) : IRequest<AccountTokenDto>;

