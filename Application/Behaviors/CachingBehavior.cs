using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Application.Behaviors;

public sealed class CachingBehavior<TRequest, TResponse>(
    HybridCache cache,
    ILogger<CachingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableQuery<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        if (string.IsNullOrWhiteSpace(request.CacheKey))
        {
            logger.LogWarning(
                "Skipping cache for {RequestName} because cache key is empty.",
                requestName);

            return await next();
        }

        logger.LogDebug(
            "Checking cache for {RequestName}. CacheKey: {CacheKey}",
            requestName,
            request.CacheKey);

        var options = new HybridCacheEntryOptions
        {
            Expiration = request.CacheOptions.AbsoluteExpiration,
            LocalCacheExpiration = request.CacheOptions.SlidingExpiration
        };

        var tags = request.Tags.Count > 0
            ? request.Tags
            : null;

        return await cache.GetOrCreateAsync(
            request.CacheKey,
            async _ =>
            {
                logger.LogDebug(
                    "Cache miss for {RequestName}. CacheKey: {CacheKey}",
                    requestName,
                    request.CacheKey);

                return await next();
            },
            options,
            tags,
            cancellationToken);
    }
}