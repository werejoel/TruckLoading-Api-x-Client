using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace TruckLoadingApp.API.Configuration;

public static class RateLimitingConfiguration
{
    public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        // Fixed window rate limiter - limits requests within a fixed time window
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("fixed", config =>
            {
                config.PermitLimit = 100;
                config.Window = TimeSpan.FromMinutes(1);
                config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                config.QueueLimit = 2;
            });

            // Sliding window rate limiter - more granular control over time windows
            options.AddSlidingWindowLimiter("sliding", config =>
            {
                config.PermitLimit = 100;
                config.Window = TimeSpan.FromMinutes(1);
                config.SegmentsPerWindow = 6; // Divides the window into 6 segments (10 seconds each)
                config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                config.QueueLimit = 2;
            });

            // Token bucket rate limiter - allows for bursts of traffic
            options.AddTokenBucketLimiter("token", config =>
            {
                config.TokenLimit = 100;
                config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                config.QueueLimit = 2;
                config.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
                config.TokensPerPeriod = 20;
                config.AutoReplenishment = true;
            });

            // Global rate limit configuration
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 200,
                        Window = TimeSpan.FromMinutes(1)
                    });
            });

            // OnRejected handler
            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.Headers["Retry-After"] = "60";
                await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", token);
            };
        });

        return services;
    }
}