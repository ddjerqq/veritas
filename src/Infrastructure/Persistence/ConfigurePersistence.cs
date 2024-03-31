using Application.Abstractions;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: HostingStartup(typeof(ConfigurePersistence))]

namespace Infrastructure.Persistence;

public class ConfigurePersistence : IHostingStartup
{
    private static bool _configured;

    public void Configure(IWebHostBuilder builder)
    {
        if (_configured) return;
        _configured = true;

        builder.ConfigureServices((context, services) =>
        {
            services.AddSingleton<ConvertDomainEventsToOutboxMessagesInterceptor>();

            services.AddDbContext<AppDbContext>(options =>
            {
                if (context.HostingEnvironment.IsDevelopment())
                {
                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                }

                var dbPath = Environment.GetEnvironmentVariable("DB__PATH") ?? "C:/work/mieci/app.db";

                options.UseSqlite($"Data Source={dbPath}");
            });

            services.AddDatabaseDeveloperPageExceptionFilter();

            // delegate the IDbContext to the AppDbContext;
            services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        });
    }
}
