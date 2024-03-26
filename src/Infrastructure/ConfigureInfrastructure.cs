using System.Threading.RateLimiting;
using Infrastructure;
using Infrastructure.Idempotency;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;

[assembly: HostingStartup(typeof(ConfigureInfrastructure))]

namespace Infrastructure;

public class ConfigureInfrastructure : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddHttpContextAccessor();
            services.AddIdempotency();
            services.AddMemoryCache();

            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.OnRejected = OnRejected;
                options.GlobalLimiter = GlobalRateLimiter;
            });
        });
    }

    private static readonly TokenBucketRateLimiterOptions GlobalPolicy = new()
    {
        AutoReplenishment = true,
        QueueLimit = int.Parse(Environment.GetEnvironmentVariable("RATE_LIMIT__QUEUE_LIMIT") ?? "10"),
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        ReplenishmentPeriod = TimeSpan.FromSeconds(int.Parse(Environment.GetEnvironmentVariable("RATE_LIMIT__REPLENISHMENT_PERIOD") ?? "5")),
        TokenLimit = int.Parse(Environment.GetEnvironmentVariable("RATE_LIMIT__TOKEN_LIMIT") ?? "50"),
        TokensPerPeriod = int.Parse(Environment.GetEnvironmentVariable("RATE_LIMIT__TOKENS_PER_PERIOD") ?? "10"),
    };

    private static readonly PartitionedRateLimiter<HttpContext> GlobalRateLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
        context =>
        {
            // TODO get the public key if possible.
            // try get the key from the ip address, or the connection id
            var key = context.Connection.RemoteIpAddress?.ToString() ?? context.Connection.Id;

            if (context.User is { Identity.IsAuthenticated: true, Claims: var claims })
            {
                var id = claims
                    .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid)?
                    .Value;

                if (!string.IsNullOrWhiteSpace(id))
                    key = id;
            }

            return RateLimitPartition.GetTokenBucketLimiter(key, _ => GlobalPolicy);
        });

    private static ValueTask OnRejected(OnRejectedContext ctx, CancellationToken ct)
    {
        if (ctx.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            ctx.HttpContext.Response.Headers.RetryAfter = retryAfter.ToString("R");

        return ValueTask.CompletedTask;
    }
}