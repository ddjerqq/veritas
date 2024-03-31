using dotenv.net;
using Presentation;
using Presentation.Config;

// fix postgres timestamp issue
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var solutionDir = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent;

DotEnv.Fluent()
    .WithTrimValues()
    .WithEnvFiles($"{solutionDir}/.env")
    .WithOverwriteExistingVars()
    .Load();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseConfiguredSerilog();

builder.WebHost.UseStaticWebAssets();
builder.WebHost.ConfigureAssemblies();

var app = builder.Build();

app.UseConfiguredSerilogRequestLogging();
app.MigrateDatabase();
app.UseExceptionHandler();

app.UseRateLimiter();

if (app.Environment.IsDevelopment())
    app.UseDevelopmentMiddleware();

if (app.Environment.IsProduction())
    app.UseProductionMiddleware();

app.UseAppMiddleware();
app.MapEndpoints();

app.Run();