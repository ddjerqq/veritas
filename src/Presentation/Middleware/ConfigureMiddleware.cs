using Presentation.Middleware;

[assembly: HostingStartup(typeof(ConfigureMiddleware))]

namespace Presentation.Middleware;

public class ConfigureMiddleware : IHostingStartup
{
    private static bool _configured;

    public void Configure(IWebHostBuilder builder)
    {
        if (_configured) return;
        _configured = true;

        builder.ConfigureServices(services =>
        {
            services.AddTransient<ProprietaryHeaderMiddleware>();
        });
    }
}