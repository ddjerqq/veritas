using System.Threading.RateLimiting;
using Application.Abstractions;
using Domain.ValueObjects;
using Infrastructure;
using Infrastructure.Idempotency;
using Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(ConfigureInfrastructure))]

namespace Infrastructure;

public class ConfigureInfrastructure : IHostingStartup
{
    private static bool _configured;

    public void Configure(IWebHostBuilder builder)
    {
        if (_configured) return;
        _configured = true;

        builder.ConfigureServices(services =>
        {
            services.AddHttpContextAccessor();
            services.AddIdempotency();
            services.AddMemoryCache();

            services.AddScoped<ICurrentVoterAccessor, CurrentVoterAccessor>();

            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.OnRejected = OnRejected;
                options.GlobalLimiter = GlobalRateLimiter;
            });
        });
    }

    private static TokenBucketRateLimiterOptions GlobalPolicy => new()
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
            string key;

            if (context.Items.TryGetValue(nameof(Voter), out var value) && value is Voter voter)
            {
                key = voter.Address;
            }
            else
            {
                key = context.Connection.RemoteIpAddress?.ToString() ?? context.Connection.Id;
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