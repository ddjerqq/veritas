using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Votes.Events;

public class BlockMinedEventHandler : INotificationHandler<BlockMinedEvent>
{
    public Task Handle(BlockMinedEvent notification, CancellationToken ct)
    {
        throw new NotImplementedException("purely for notifications with SignalR");
    }
}