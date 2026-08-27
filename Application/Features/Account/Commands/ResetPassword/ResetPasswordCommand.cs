using MediatR;

namespace Application.Features.Account.Commands.ResetPassword;

public record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest;

