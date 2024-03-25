using Domain.Events;
using MediatR;

namespace Application.Votes.Events;

public class UserVotedEventHandler : INotificationHandler<UserVotedEvent>
{
    public Task Handle(UserVotedEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}