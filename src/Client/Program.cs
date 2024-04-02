using System.Text.Json;
using System.Text.Json.Serialization;
using Blazored.LocalStorage;
using Blazored.Toast;
using Client;
using Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

if (builder.HostEnvironment.IsProduction())
    builder.Logging.SetMinimumLevel(LogLevel.None);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/v1/") });
builder.Services.AddSingleton(builder.HostEnvironment);
builder.Services.AddScoped<AuthenticationStateProvider, PublicKeyAuthStateProvider>();

builder.Services.AddBlazoredToast();
builder.Services.AddBlazoredLocalStorageAsSingleton(o =>
{
    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

await app.RunAsync();