using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Votes.Events;

public class BlockMinedEventHandler(ILogger<BlockMinedEventHandler> logger) : INotificationHandler<BlockMinedEvent>
{
    // purely for notifications with SignalR
    public Task Handle(BlockMinedEvent notification, CancellationToken ct)
    {
        logger.LogInformation(notification.ToString());
        return Task.CompletedTask;
    }
}