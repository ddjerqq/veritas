using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Common.Abstractions;

[ApiController]
[Produces("application/json")]
public abstract class ApiController : ControllerBase
{
    protected T GetService<T>() where T : notnull => HttpContext.RequestServices.GetRequiredService<T>();

    protected IMediator Mediator => GetService<IMediator>();
}