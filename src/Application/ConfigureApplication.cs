using Application;
using Application.Abstractions;
using Application.Behaviours;
using Application.Services;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(ConfigureApplication))]

namespace Application;

public class ConfigureApplication : IHostingStartup
{
    private static bool _configured;

    public void Configure(IWebHostBuilder builder)
    {
        if (_configured) return;
        _configured = true;

        builder.ConfigureServices(services =>
        {
            services.AddScoped<IDateTimeProvider, UtcDateTimeProvider>();
            services.AddInMemoryProcessedVoteCache();

            services.AddAutoMapper(mapper => mapper.AddMaps(ApplicationAssembly.Assembly));

            services.AddMediatR(options =>
            {
                options.RegisterServicesFromAssembly(ApplicationAssembly.Assembly);
                options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
                options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehaviour<,>));
            });
        });
    }
}