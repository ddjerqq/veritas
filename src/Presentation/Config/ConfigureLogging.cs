using Microsoft.AspNetCore.HttpLogging;
using Presentation.Config;

[assembly: HostingStartup(typeof(ConfigureLogging))]

namespace Presentation.Config;

public class ConfigureLogging : IHostingStartup
{
    private static bool _configured;

    public void Configure(IWebHostBuilder builder)
    {
        if (_configured) return;
        _configured = true;

        builder.ConfigureServices(services =>
        {
            services.AddLogging(loggingBuilder =>
            {
                // TODO optimization add file logging, for everything that is happening.
                loggingBuilder.AddSimpleConsole(options =>
                {
                    options.SingleLine = false;
                    options.IncludeScopes = false;
                    options.UseUtcTimestamp = true;
                    options.TimestampFormat = "[dd-MM-yyyyThh:mm:ss.fff] ";
                });
            });

            services.AddHttpLogging(options =>
            {
                options.LoggingFields =
                    HttpLoggingFields.RequestPath
                    | HttpLoggingFields.ResponseStatusCode
                    | HttpLoggingFields.RequestMethod
                    | HttpLoggingFields.RequestQuery
                    | HttpLoggingFields.RequestHeaders
                    | HttpLoggingFields.ResponseHeaders;

                options.RequestHeaders.Add("X-Idempotency-Key");
                options.ResponseHeaders.Add("X-Client-IP");
                options.ResponseHeaders.Add("X-Response-Time");

                options.RequestBodyLogLimit = 4096;
                options.ResponseBodyLogLimit = 4096;
            });
        });
    }
}