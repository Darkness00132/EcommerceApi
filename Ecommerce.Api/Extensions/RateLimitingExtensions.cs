using System.Threading.RateLimiting;
using Ecommerce.Api.Constants;

namespace Ecommerce.Api.Extensions;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddRateLimiting(
        this IServiceCollection services)
    {
        services.AddRateLimiter(options => {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter = CreateLimiter(
                120,
                TimeSpan.FromMinutes(1));

            options.AddPolicy(
                RateLimitApiConstants.AuthPolicy,
                context => CreateLimiter(
                    context,
                    10,
                    TimeSpan.FromMinutes(1)));
        });

        return services;
    }

    private static PartitionedRateLimiter<HttpContext> CreateLimiter(
        int permitLimit,
        TimeSpan window)
    {
        return PartitionedRateLimiter.Create<HttpContext, string>(
            context => CreateLimiter(
                context,
                permitLimit,
                window));
    }

    private static RateLimitPartition<string> CreateLimiter(
        HttpContext context,
        int permitLimit,
        TimeSpan window)
    {
        var clientIp = GetClientIp(context);

        return RateLimitPartition.GetFixedWindowLimiter(
            clientIp,
            _ => new FixedWindowRateLimiterOptions {
                PermitLimit = permitLimit,
                Window = window,
                QueueLimit = 0
            });
    }

    private static string GetClientIp(HttpContext context)
    {
        return context.Connection.RemoteIpAddress?.ToString()
               ?? "unknown";
    }
}
