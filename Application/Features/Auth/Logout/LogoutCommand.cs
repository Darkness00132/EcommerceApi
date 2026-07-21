using MediatR;

namespace Application.Features.Auth.Logout
{
    public record LogoutCommand(string token) : IRequest;
}
