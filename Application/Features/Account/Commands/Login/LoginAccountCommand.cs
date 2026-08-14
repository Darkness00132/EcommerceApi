using Application.Features.Account.Dto;
using MediatR;

namespace Application.Features.Account.Commands.Login;

public sealed record LoginAccountCommand(string Email, string Password) : IRequest<AccountTokenDto>;

