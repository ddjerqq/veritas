using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Blockchain.Events;

// ReSharper disable once UnusedType.Global
public sealed class VoteAddedEventHandler(ILogger<VoteAddedEventHandler> logger) : INotificationHandler<VoteAddedEvent>
{
    public Task Handle(VoteAddedEvent notification, CancellationToken ct)
    {
        // TODO implement
        logger.LogInformation("not implemented");
        return Task.CompletedTask;
    }
}