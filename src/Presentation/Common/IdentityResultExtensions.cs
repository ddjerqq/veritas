using System.Diagnostics;
using Domain.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Common;

public static class IdentityResultExtensions
{
    public static ProblemDetails ToProblemDetails(this IdentityResult result)
    {
        Debug.Assert(!result.Succeeded);

        var problemDetails = new ProblemDetails
        {
            Title = "One or more identity errors have occurred.",
            Status = 400,
            Type = "https://httpstatuses.com/400",
            Instance = "https://httpstatuses.com/400",
            Extensions =
            {
                ["traceId"] = Activity.Current?.Id ?? "unknown",
                ["errors"] = result.Errors.Select(error => new
                {
                    Code = error.Code.ToSnakeCase(),
                    error.Description,
                }),
            },
        };

        return problemDetails;
    }
}