using Application.Common.Abstractions;
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

    private static string ConnectionString
    {
        get
        {
            var env = Environment.GetEnvironmentVariables();

            var host = env["POSTGRES__HOST"] ?? "postgres";
            var port = env["POSTGRES__PORT"] as int? ?? 5432;
            var user = env["POSTGRES__USER"] ?? "postgres";
            var db = env["POSTGRES__DB"] ?? "postgres";
            var pass = env["POSTGRES__PASS"] ??
                       throw new ArgumentNullException(nameof(db), "POSTGRES__PASS is not set in the environment.");

            return $"Host={host};Database={db};Port={port};Username={user};Password={pass};Include Error Detail=true";
        }
    }

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

                // var dbPath = Environment.GetEnvironmentVariable("DB__PATH") ?? throw new Exception("DB__PATH is not set");
                // options.UseSqlite($"Data Source={dbPath}");

                options.UseNpgsql(ConnectionString);
            });

            services.AddDatabaseDeveloperPageExceptionFilter();

            // delegate the IDbContext to the AppDbContext;
            services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        });
    }
}