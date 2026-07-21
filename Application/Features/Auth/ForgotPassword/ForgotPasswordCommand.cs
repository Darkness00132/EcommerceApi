using MediatR;

namespace Application.Features.Auth.ForgotPassword
{
    public record ForgotPasswordCommand(string Email) : IRequest;
}
