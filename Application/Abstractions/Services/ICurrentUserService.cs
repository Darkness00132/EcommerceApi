namespace Application.Abstractions.Services;

public interface ICurrentUserService
{
    Guid UserId { get; }

    string Email { get; }

    IReadOnlyList<string> Roles { get; }

    bool IsInRole(string role);

    bool IsAuthenticated { get; }
}
