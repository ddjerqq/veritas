using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Votes.Events;

public class BlockMinedEventHandler(ILogger<BlockMinedEventHandler> logger) : INotificationHandler<BlockMinedEvent>
{
    public Task Handle(BlockMinedEvent notification, CancellationToken ct)
    {
        throw new NotImplementedException("purely for notifications with SignalR");
    }
}