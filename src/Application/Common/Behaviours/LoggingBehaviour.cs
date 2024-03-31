using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviours;

internal sealed class LoggingBehaviour<TRequest, TResponse>(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        logger.LogInformation("started request {@RequestName} {@Request}", typeof(TRequest).Name, request);

        var stopwatch = Stopwatch.StartNew();
        var result = await next();
        stopwatch.Stop();

        logger.LogInformation("finished request {@RequestName} {@Request} in {@Duration}ms",
            typeof(TRequest).Name,
            request,
            stopwatch.ElapsedMilliseconds);

        return result;
    }
}