using System.Diagnostics;
using Application.Abstractions;
using Application.Votes.Events;
using Domain.Entities;
using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Votes.Commands;

public record MineCurrentBlockCommand : IRequest<Block>;

public class MineCurrentBlockCommandHandler(
    IAppDbContext dbContext,
    IBlockCache blockCache,
    IDateTimeProvider dateTimeProvider,
    ILogger<VoteAddedEventHandler> logger)
    : IRequestHandler<MineCurrentBlockCommand, Block>
{
    public async Task<Block> Handle(MineCurrentBlockCommand request, CancellationToken ct)
    {
        var currentBlock = await blockCache.GetCurrentAsync(ct);

        // MINE AND TIME
        var stopwatch = Stopwatch.StartNew();
        currentBlock.Mine();
        stopwatch.Stop();
        logger.LogInformation("new block mined in {Elapsed:c}", stopwatch.Elapsed);

        dbContext.Set<Block>().Update(currentBlock);

        var next = currentBlock.NextBlock();
        blockCache.SetCurrent(next);
        dbContext.Set<Block>().Add(next);

        var blockMinedEvent = new BlockMinedEvent(currentBlock.Index);
        dbContext.AddDomainEvent(blockMinedEvent, dateTimeProvider);

        await dbContext.SaveChangesAsync(ct);

        return next;
    }
}