using MediatR;

namespace Application.Features.Auth.ConfirmEmail
{
    public record ConfirmEmailCommand(Guid UserId, string Token) : IRequest;
}
