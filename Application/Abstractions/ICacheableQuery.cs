using MediatR;

public interface ICacheableQuery<TResponse>
    : IRequest<TResponse>
{
    string CacheKey { get; }

    IReadOnlyCollection<string> Tags { get; }

    CacheOptions CacheOptions { get; }
    bool BypassCache { get; }
}

public sealed record CacheOptions
{
    public TimeSpan AbsoluteExpiration { get; init; }

    public TimeSpan? SlidingExpiration { get; init; }
}
