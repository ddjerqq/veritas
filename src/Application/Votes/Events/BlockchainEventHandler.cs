using Domain.Events;
using MediatR;

namespace Application.Votes.Events;

public class BlockchainEventHandler : INotificationHandler<VoteAddedEvent>, INotificationHandler<BlockMinedEvent>
{
    public Task Handle(VoteAddedEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Handle(BlockMinedEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}