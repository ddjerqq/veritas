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

        builder.ConfigureServices(services =>
        {
            services.AddExceptionHandler<ValidationExceptionHandler>();
            services.AddExceptionHandler<GlobalExceptionHandler>();

            services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = ctx =>
                {
                    ctx.ProblemDetails.Type = $"https://httpstatuses.com/{ctx.ProblemDetails.Status}";
                    ctx.ProblemDetails.Extensions["addr"] = ctx.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "addr");
                    ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
                };
            });
        });
    }
}