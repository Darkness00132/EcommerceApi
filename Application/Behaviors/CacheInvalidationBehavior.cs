using Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

public sealed class CacheInvalidationBehavior<TRequest, TResponse>(
    HybridCache cache,
    ILogger<CacheInvalidationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheInvalidatingCommand<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next();

        await InvalidateCacheAsync(request, cancellationToken);

        return response;
    }

    private async Task InvalidateCacheAsync(
        ICacheInvalidatingCommand<TResponse> request,
        CancellationToken cancellationToken)
    {
        foreach (var cacheKey in request.CacheKeys.Where(key => !string.IsNullOrWhiteSpace(key)).Distinct())
        {
            try
            {
                await cache.RemoveAsync(cacheKey, cancellationToken);

                logger.LogDebug(
                    "Cache key removed successfully. Key: {CacheKey}",
                    cacheKey);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger.LogWarning(
                    exception,
                    "Failed to remove cache key. Key: {CacheKey}, Request: {RequestType}",
                    cacheKey,
                    typeof(TRequest).Name);
            }
        }

        foreach (var cacheTag in request.CacheTags.Where(tag => !string.IsNullOrWhiteSpace(tag)).Distinct())
        {
            try
            {
                await cache.RemoveByTagAsync(cacheTag, cancellationToken);

                logger.LogDebug(
                    "Cache tag removed successfully. Tag: {CacheTag}",
                    cacheTag);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger.LogWarning(
                    exception,
                    "Failed to remove cache tag. Tag: {CacheTag}, Request: {RequestType}",
                    cacheTag,
                    typeof(TRequest).Name);
            }
        }
    }
}