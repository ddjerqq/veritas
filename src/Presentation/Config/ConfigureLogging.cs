using Domain.ValueObjects;
using Microsoft.AspNetCore.HttpLogging;
using Presentation.Config;
using Serilog;
using Serilog.Events;

[assembly: HostingStartup(typeof(ConfigureLogging))]

namespace Presentation.Config;

public class ConfigureLogging : IHostingStartup
{
    private static bool _configured;

    public const string OutputFormat = "[{Timestamp:dd-MM-yyyy HH:mm:ss.fff} {Level:u3}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}";

    private static string LogDir => Environment.GetEnvironmentVariable("LOG__DIRECTORY")?.TrimEnd('/') ?? "/var/log/veritas";

    public static string LogPath => $"{LogDir}/veritas-{DateTime.UtcNow:s}.log";

    public void Configure(IWebHostBuilder builder)
    {
        if (_configured) return;
        _configured = true;

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Quartz", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Debug()
            .WriteTo.Console(outputTemplate: OutputFormat)
            .WriteTo.File(LogPath,
                flushToDiskInterval: TimeSpan.FromSeconds(10),
                outputTemplate: OutputFormat,
                fileSizeLimitBytes: 100_000_000,
                rollOnFileSizeLimit: true,
                rollingInterval: RollingInterval.Day)
            .CreateLogger();

        builder.ConfigureServices(services =>
        {
            services.AddSerilog();
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(Log.Logger, true));
        });
    }
}

public static class WebAppExtensions
{
    public static void UseConfiguredSerilogRequestLogging(this IApplicationBuilder app)
    {
        // TODO add these fields
        // services.AddHttpLogging(options =>
        // {
        //     options.LoggingFields =
        //         HttpLoggingFields.RequestPath
        //         | HttpLoggingFields.ResponseStatusCode
        //         | HttpLoggingFields.RequestMethod
        //         | HttpLoggingFields.RequestQuery
        //         | HttpLoggingFields.RequestHeaders
        //         | HttpLoggingFields.ResponseHeaders;
        //
        //     options.RequestHeaders.Add("X-Idempotency-Key");
        //     options.ResponseHeaders.Add("X-Client-IP");
        //     options.ResponseHeaders.Add("X-Response-Time");
        //
        //     options.RequestBodyLogLimit = 4096;
        //     options.ResponseBodyLogLimit = 4096;
        // });

        app.UseSerilogRequestLogging(options =>
        {
            options.IncludeQueryInRequestPath = true;
            options.MessageTemplate = "{RequestMethod} {RequestPath}";

            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                if (httpContext.Items.TryGetValue(nameof(Voter), out var value) && value is Voter voter)
                    diagnosticContext.Set("VoterAddress", voter.Address);

                diagnosticContext.Set("RequestUserAgent", httpContext.Request.Headers.UserAgent);
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            };
        });
    }

    public static void UseConfiguredSerilog(this ConfigureHostBuilder host)
    {
        host.UseSerilog((_, configuration) =>
        {
            configuration
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Quartz", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Debug()
                .WriteTo.Console(outputTemplate: ConfigureLogging.OutputFormat)
                .WriteTo.File(ConfigureLogging.LogPath,
                    flushToDiskInterval: TimeSpan.FromSeconds(10),
                    outputTemplate: ConfigureLogging.OutputFormat,
                    fileSizeLimitBytes: 100_000_000,
                    rollOnFileSizeLimit: true,
                    rollingInterval: RollingInterval.Day);
        });
    }
}