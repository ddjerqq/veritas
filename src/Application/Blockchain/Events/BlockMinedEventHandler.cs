using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Blockchain.Events;

// ReSharper disable once UnusedType.Global
public sealed class BlockMinedEventHandler(ILogger<BlockMinedEventHandler> logger) : INotificationHandler<BlockMinedEvent>
{
    public Task Handle(BlockMinedEvent notification, CancellationToken ct)
    {
        // TODO implement
        logger.LogInformation("not implemented");
        return Task.CompletedTask;
    }
}