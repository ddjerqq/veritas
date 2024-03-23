using System.Text.Json;
using System.Text.Json.Serialization;
using Api.Data;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.JsonSerializerOptions.AllowTrailingCommas = true;
    });

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var dbPath = Environment.GetEnvironmentVariable("DB__PATH") ?? "C:/work/mieci/app.db";
    options.UseSqlite($"Data Source={dbPath}");
});

builder.Services.AddResponseCaching();
builder.Services.AddResponseCompression(options =>
{
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/octet-stream"]);
    options.Providers.Add<GzipCompressionProvider>();
    options.Providers.Add<BrotliCompressionProvider>();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCompression();
app.UseResponseCaching();

app.MapControllers();
app.Run();

// program summary:
// user must mint a token before they can see the votes
// the token will be a key, which will decrypt the public ledger
// there can exist many keys like this one.