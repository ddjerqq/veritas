using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Votes.Events;

public class VoteAddedEventHandler(ILogger<BlockMinedEventHandler> logger) : INotificationHandler<VoteAddedEvent>
{
    public Task Handle(VoteAddedEvent notification, CancellationToken ct)
    {
        var vote = notification.GetVote();
        logger.LogInformation(vote.ToString());
        logger.LogInformation(notification.ToString());
        return Task.CompletedTask;
    }
}