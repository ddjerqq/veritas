using System.Net;
using Application.Common.Abstractions;
using Application.Services;
using Blazored.LocalStorage;
using Blazored.Modal;
using Blazored.Toast;
using Client;
using Client.Common;
using Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

if (builder.HostEnvironment.IsProduction())
    builder.Logging.SetMinimumLevel(LogLevel.None);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IDateTimeProvider, UtcDateTimeProvider>();

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<CookieUtil>();
builder.Services.AddScoped<VoterAccessor>();
builder.Services.AddScoped<VoteService>();

builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped(_ =>
{
    // TODO CREDENTIALS ARE NOT BEING SENT, FIX CORS, ADD LOCALHOST:5001 DURING TESTING
    var credentials = new HttpClientHandler() { Credentials = new NetworkCredential() };
    return new HttpClient { BaseAddress = new Uri("https://localhost/") };
});
builder.Services.AddSingleton(builder.HostEnvironment);
builder.Services.AddScoped<AuthenticationStateProvider, PublicKeyAuthStateProvider>();

builder.Services.AddBlazoredToast();
builder.Services.AddBlazoredModal();
builder.Services.AddBlazoredLocalStorageAsSingleton(o => { o.JsonSerializerOptions = Json.SerializerOptions; });

var app = builder.Build();

await app.RunAsync();