using System.Security.Claims;
using Application.Abstractions.Services;
using Application.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Api.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated ?? false;

    public Guid UserId
    {
        get
        {
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(userId, out var id)
                ? id
                : throw new UnauthorizedException();
        }
    }

    public string Email =>
        User?.FindFirstValue(ClaimTypes.Email)
        ?? string.Empty;

    public IReadOnlyList<string> Roles =>
        User?
            .FindAll(ClaimTypes.Role)
            .Select(x => x.Value)
            .ToList()
        ?? [];

    public bool IsInRole(string role) =>
        User?.IsInRole(role) ?? false;
}