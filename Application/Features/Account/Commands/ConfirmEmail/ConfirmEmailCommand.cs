using MediatR;

namespace Application.Features.Account.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommand(string Email, string Token) : IRequest;

