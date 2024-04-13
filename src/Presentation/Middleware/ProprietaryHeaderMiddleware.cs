using Microsoft.Extensions.Primitives;

namespace Presentation.Middleware;

public sealed class ProprietaryHeaderMiddleware : IMiddleware
{
    private static readonly Dictionary<string, StringValues> Headers = new()
    {
        // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options
        ["X-Frame-Options"] = "DENY",
        // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Content-Type-Options
        ["X-Content-Type-Options"] = "nosniff",
    };

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        foreach (var header in Headers)
        {
            context.Response.Headers.Add(header);
        }

        return next(context);
    }
}

public static class ProprietaryHeaderMiddlewareExt
{
    public static void UseProprietaryHeaders(this WebApplication app)
    {
        app.UseMiddleware<ProprietaryHeaderMiddleware>();
    }
}