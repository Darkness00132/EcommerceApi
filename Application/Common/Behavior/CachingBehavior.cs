using System.Text.Json;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behavior
{
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICacheableQuery<TResponse>
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

        // System.Text.Json configuration (preserves property casing and handles modern C# records)
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };

        public CachingBehavior(
            IDistributedCache cache,
            ILogger<CachingBehavior<TRequest, TResponse>> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            string requestName = typeof(TRequest).Name;
            string cacheKey = request.CacheKey;

            // 0. Skip caching completely if the query opts out (empty key)
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                _logger.LogInformation(
                    "Caching SKIPPED for {RequestName}: CacheKey is empty or intentionally bypassed.",
                    requestName);

                return await next();
            }

            // 1. Try reading from Redis with graceful exception handling
            try
            {
                string? cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);

                if (!string.IsNullOrEmpty(cachedData))
                {
                    _logger.LogInformation(
                        "Cache HIT for {RequestName} using key: {CacheKey}",
                        requestName,
                        cacheKey);

                    var deserializedResponse = JsonSerializer.Deserialize<TResponse>(cachedData, JsonOptions);

                    if (deserializedResponse is not null)
                    {
                        return deserializedResponse;
                    }
                }
            }
            catch (Exception ex)
            {
                // Fallback gracefully: If Redis drops or has network issues, log the warning and let the DB query run instead of crashing the API
                _logger.LogWarning(
                    ex,
                    "Failed to fetch key {CacheKey} from Redis for {RequestName}. Falling back to database handler.",
                    cacheKey,
                    requestName);
            }

            _logger.LogInformation(
                "Cache MISS for {RequestName} using key: {CacheKey}. Executing handler...",
                requestName,
                cacheKey);

            // 2. Fetch data from Database / Inner Handler
            TResponse response = await next();

            // 3. Save response to Redis
            if (response is not null)
            {
                try
                {
                    var cacheOptions = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = request.AbsoluteExpiration,
                        SlidingExpiration = request.SlidingExpiration
                    };

                    string serializedData = JsonSerializer.Serialize(response, JsonOptions);

                    await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions, cancellationToken);

                    _logger.LogInformation(
                        "Successfully cached {RequestName} under key: {CacheKey}",
                        requestName,
                        cacheKey);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        ex,
                        "Failed to save key {CacheKey} to Redis for {RequestName}.",
                        cacheKey,
                        requestName);
                }
            }

            return response;
        }
    }
}