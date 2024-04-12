using Domain.Entities;
using Presentation.Config;
using Serilog;
using Serilog.Events;

[assembly: HostingStartup(typeof(ConfigureLogging))]

namespace Presentation.Config;

public class ConfigureLogging : IHostingStartup
{
    private static bool _configured;

    public void Configure(IWebHostBuilder builder)
    {
        if (_configured) return;
        _configured = true;

        Log.Logger = new LoggerConfiguration()
            .Configure()
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
    private const string OutputFormat =
        "[{Timestamp:dd-MM-yyyy HH:mm:ss.fff} {Level:u3}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}";

    public static LoggerConfiguration Configure(this LoggerConfiguration config)
    {
        var logPath = Environment.GetEnvironmentVariable("LOG__PATH") ?? ".logs/veritas-.log";

        return config
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Quartz", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId()
            .Enrich.WithAssemblyName()
            .WriteTo.Debug()
            .WriteTo.Console(outputTemplate: OutputFormat)
            .WriteTo.File(logPath,
                outputTemplate: OutputFormat,
                flushToDiskInterval: TimeSpan.FromSeconds(10),
                fileSizeLimitBytes: 100_000_000,
                rollOnFileSizeLimit: true,
                rollingInterval: RollingInterval.Day);
    }

    public static void UseConfiguredSerilogRequestLogging(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.IncludeQueryInRequestPath = true;
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms\n" +
                                      "Address: {VoterAddress}\n" +
                                      "Host: {RequestHost}\n" +
                                      "UserAgent: {RequestUserAgent}";

            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("VoterAddress", "NaN");
                if (httpContext.Items.TryGetValue(nameof(Voter), out var value) && value is Voter { Address: var address })
                    diagnosticContext.Set("VoterAddress", address);

                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestUserAgent", (string?)httpContext.Request.Headers.UserAgent);
            };
        });
    }

    public static void UseConfiguredSerilog(this ConfigureHostBuilder host)
    {
        host.UseSerilog((_, configuration) => configuration.Configure());
    }
}