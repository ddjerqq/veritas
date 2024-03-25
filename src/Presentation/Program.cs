using dotenv.net;
using Presentation;

// fix postgres timestamp issue
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

DotEnv.Fluent()
    .WithTrimValues()
    .WithOverwriteExistingVars()
    .WithProbeForEnv(5)
    .Load();

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseStaticWebAssets();
builder.WebHost.ConfigureAssemblies();

var app = builder.Build();

app.MigrateDatabase();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
    app.UseDevelopmentMiddleware();

if (app.Environment.IsProduction())
    app.UseProductionMiddleware();

app.UseAppMiddleware();
app.MapEndpoints();

app.Run();