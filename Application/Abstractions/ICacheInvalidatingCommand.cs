using MediatR;

namespace Application.Abstractions;

public interface ICacheInvalidatingCommand<TResponse> : IRequest<TResponse>
{
    IReadOnlyCollection<string> CacheKeys { get; }

    IReadOnlyCollection<string> CacheTags { get; }
}

public interface ICacheInvalidatingCommand : IRequest
{
    IReadOnlyCollection<string> CacheKeys { get; }

    IReadOnlyCollection<string> CacheTags { get; }
}
