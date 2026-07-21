using MediatR;

namespace Application.Features.Auth.ResetPassword
{
    public record ResetPasswordCommand(string Email,string Token , string NewPassword) : IRequest;
}
