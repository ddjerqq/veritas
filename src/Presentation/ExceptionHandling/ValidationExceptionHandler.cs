using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.ExceptionHandling;

public sealed class ValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        if (exception is ValidationException validationException)
        {
            var errors = validationException
                .Errors
                .ToDictionary(
                    e => e.ErrorCode,
                    e => new [] { e.ErrorMessage });

            var problemDetails = new ValidationProblemDetails(errors);
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, ct);
            return true;
        }

        return false;
    }
}