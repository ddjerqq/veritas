using System.Diagnostics;
using Application.Abstractions;
using Application.Dtos;
using Application.Votes.Events;
using Domain.Aggregates;
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
        var currentBlockDto = await blockCache.GetCurrentAsync(ct);
        var currentBlock = (Block)currentBlockDto;

        // MINE AND TIME
        var stopwatch = Stopwatch.StartNew();
        currentBlock = currentBlock.Mine();
        stopwatch.Stop();
        logger.LogInformation("new block mined in {Elapsed:c}", stopwatch.Elapsed);

        currentBlockDto.CopyFrom(currentBlock);
        dbContext.Set<BlockDto>().Update(currentBlockDto);

        var next = currentBlock.Next();
        blockCache.SetCurrent(next);
        dbContext.Set<BlockDto>().Add(next);

        var blockMinedEvent = new BlockMinedEvent(currentBlock.Index);
        dbContext.AddDomainEvent(blockMinedEvent, dateTimeProvider);

        await dbContext.SaveChangesAsync(ct);

        return next;
    }
}