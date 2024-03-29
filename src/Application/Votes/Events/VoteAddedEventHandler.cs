using Domain.Events;
using MediatR;

namespace Application.Votes.Events;

public class VoteAddedEventHandler : INotificationHandler<VoteAddedEvent>
{
    public Task Handle(VoteAddedEvent notification, CancellationToken ct)
    {
        throw new NotImplementedException("TODO SignalR - trigger SignalR");
    }
}