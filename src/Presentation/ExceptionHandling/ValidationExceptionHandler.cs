using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.ExceptionHandling;

public sealed class ValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        if (exception is not ValidationException validationException) return false;

        var errors = validationException
            .Errors
            .ToDictionary(
                e => e.ErrorCode,
                e => new[] { e.ErrorMessage });

        var problemDetails = new ProblemDetails
        {
            Title = "An error occurred",
            Type = "https://httpstatuses.com/400",
            Status = StatusCodes.Status500InternalServerError,
            Detail = validationException.Message,
            Extensions =
            {
                ["addr"] = httpContext.User.Claims.FirstOrDefault(c => c.Type == "addr"),
                ["traceId"] = httpContext.TraceIdentifier,
                ["errors"] = errors,
            },
        };

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, ct);
        return true;
    }
}