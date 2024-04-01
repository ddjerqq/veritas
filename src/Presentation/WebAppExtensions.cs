using Infrastructure.Idempotency;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

namespace Presentation;

public static class WebAppExtensions
{
    public static void ConfigureAssemblies(this ConfigureWebHostBuilder builder)
    {
        // assemblies
        AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IHostingStartup).IsAssignableFrom(type))
            .Where(type => type is { IsInterface: false, IsAbstract: false })
            .Where(type => type.Name.StartsWith("configure", StringComparison.InvariantCultureIgnoreCase))
            .Select(type => (IHostingStartup)Activator.CreateInstance(type)!)
            .ToList()
            .ForEach(hostingStartup =>
            {
                var name = hostingStartup.GetType().Name.Replace("Configure", "");
                Console.WriteLine($@"[{DateTime.UtcNow:s}.000 INF] Configured {name}");
                hostingStartup.Configure(builder);
            });
    }

    public static void MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();

        dbContext.SeedGenesisBlock();
        dbContext.EnsureBlockchainIsConsistent();

        if (dbContext.Database.GetPendingMigrations().Any())
            dbContext.Database.Migrate();
    }

    public static void UseDevelopmentMiddleware(this WebApplication app)
    {
        // app.UseDeveloperExceptionPage();
        app.UseMigrationsEndPoint();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseWebAssemblyDebugging();
    }

    public static void UseProductionMiddleware(this WebApplication app)
    {
        app.UseRateLimiter();
        app.UseHsts();
        app.UseHttpsRedirection();
        app.UseIdempotency();
    }

    public static void UseAppMiddleware(this WebApplication app)
    {
        app.UseRouting();
        app.UseRequestLocalization();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseAntiforgery();

        if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SERVE_FRONTEND")))
            app.UseBlazorFrameworkFiles();

        // compress and then cache static files
        app.UseResponseCompression();
        app.UseResponseCaching();

        app.UseStaticFiles();
    }

    public static void MapEndpoints(this WebApplication app)
    {
        app.MapSwagger();

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            AllowCachingResponses = false,
        });

        app.MapControllers();
        app.MapDefaultControllerRoute();

        if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SERVE_FRONTEND")))
            app.MapFallbackToFile("index.html");
    }
}