using Presentation.Auth;
using Presentation.Config;

[assembly: HostingStartup(typeof(ConfigureAuth))]

namespace Presentation.Config;

public class ConfigureAuth : IHostingStartup
{
    private static bool _configured;

    public void Configure(IWebHostBuilder builder)
    {
        if (_configured) return;
        _configured = true;

        builder.ConfigureServices(services =>
        {
            services.AddAuthentication(PublicKeyBearerAuthHandler.SchemaName)
                .AddScheme<PublicKeyBearerSchemeOptions, PublicKeyBearerAuthHandler>(PublicKeyBearerAuthHandler.SchemaName, _ => { });
        });
    }
}