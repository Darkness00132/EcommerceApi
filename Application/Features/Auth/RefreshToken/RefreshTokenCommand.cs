using Application.Features.Auth.Dtos;
using MediatR;

namespace Application.Features.Auth.RefreshToken
{
    public record RefreshTokenCommand(string token) : IRequest<AuthResponse>;
}
