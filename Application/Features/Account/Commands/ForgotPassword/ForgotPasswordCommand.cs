using MediatR;

namespace Application.Features.Account.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : IRequest;

