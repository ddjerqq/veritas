using Application.Common.Abstractions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.ExceptionHandling;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        logger.LogError(exception, "An unhandled exception occurred. {Message}", exception.Message);

        var currentVoterAccessor = httpContext.RequestServices.GetRequiredService<ICurrentVoterAccessor>();
        var voter = currentVoterAccessor.TryGetCurrentVoter();

        var problemDetails = new ProblemDetails
        {
            Type = "https://httpstatuses.com/500",
            Title = "An error occurred",
            Status = StatusCodes.Status500InternalServerError,
            Detail = exception.Message,
            Extensions =
            {
                ["addr"] = voter?.Address,
                ["traceId"] = httpContext.TraceIdentifier,
            },
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, ct);

        return true;
    }
}