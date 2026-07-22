using MediatR;

namespace Application.Interfaces
{
    public interface ICacheableQuery<TResponse> : IRequest<TResponse>
    {
        string CacheKey { get; }
        TimeSpan? AbsoluteExpiration { get; }
        TimeSpan? SlidingExpiration { get; }
    }
}
