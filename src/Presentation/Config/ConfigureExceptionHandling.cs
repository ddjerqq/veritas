using Presentation.Config;
using Presentation.ExceptionHandling;

[assembly: HostingStartup(typeof(ConfigureExceptionHandling))]

namespace Presentation.Config;

public class ConfigureExceptionHandling : IHostingStartup
{
    private static bool _configured;

    public void Configure(IWebHostBuilder builder)
    {
        if (_configured) return;
        _configured = true;

        builder.ConfigureServices(services => services.AddTransient<GlobalExceptionHandlerMiddleware>());
    }
}

public static class ExceptionHandlingExt
{
    public static void UseGlobalExceptionHandler(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}