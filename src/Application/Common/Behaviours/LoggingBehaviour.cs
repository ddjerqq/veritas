using System.Diagnostics;
using Application.Common.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviours;

internal sealed class LoggingBehaviour<TRequest, TResponse>(
    ICurrentVoterAccessor currentVoterAccessor,
    ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var currentVoter = currentVoterAccessor.TryGetCurrentVoter();
        var address = currentVoter?.Address ?? "NaN";

        logger.LogInformation(
            "{Address} started request {@RequestName} {@Request}",
            address,
            typeof(TRequest).Name,
            request);

        var stopwatch = Stopwatch.StartNew();
        var result = await next();
        stopwatch.Stop();

        logger.LogInformation("{Address} finished request {@RequestName} {@Request} in {@Duration:c}ms",
            address,
            typeof(TRequest).Name,
            request,
            stopwatch.Elapsed);

        return result;
    }
}