using MediatR;

namespace Application.Features.Auth.Register
{
    public record RegisterCommand(
            string FirstName,
            string LastName,
            string Email,
            string Password
            ) : IRequest;
}
