using Application.Features.Auth.Dtos;
using MediatR;

namespace Application.Features.Auth.Login
{
    public record LoginCommand(string Email,string Password) : IRequest<AuthResponse>;
}
