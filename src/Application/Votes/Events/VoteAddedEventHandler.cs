using System.Diagnostics;
using Application.Abstractions;
using Application.Common;
using Application.Dtos;
using AutoMapper;
using Domain.Aggregates;
using Domain.Events;
using Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Votes.Events;

// TODO move this to CastVoteCommandHandler. because it should be handled there.
public class VoteAddedEventHandler(
    ILogger<VoteAddedEventHandler> logger,
    IMapper mapper,
    IAppDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    IBlockCache blockCache) : INotificationHandler<VoteAddedEvent>
{
    public async Task Handle(VoteAddedEvent notification, CancellationToken ct)
    {
        var currentBlock = blockCache.GetCurrent();

        if (currentBlock.Votes.Count >= Block.VoteLimit)
        {
            currentBlock = await MineBlockAsync(ct);
        }

        var vote = notification.GetVote();
        var voteDto = mapper.Map<Vote, VoteDto>(vote);
        voteDto.BlockIndex = currentBlock.Index;

        logger.LogInformation("{Address} voted for party {PartyId} at {Timestamp}", vote.Voter.Address, vote.PartyId, vote.Timestamp);

        dbContext.Set<VoteDto>().Add(voteDto);
        await dbContext.SaveChangesAsync(ct);

        currentBlock.TryAddVote(vote);

        // TODO SignalR - trigger SignalR
    }

    private async Task<Block> MineBlockAsync(CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var currentBlock = blockCache.MineAndRotate();
        stopwatch.Stop();
        logger.LogInformation("new block mined in {Elapsed:c}", stopwatch.Elapsed);

        var minedBlockDto = mapper.Map<Block, BlockDto>(currentBlock);
        dbContext.Set<BlockDto>().Add(minedBlockDto);

        var blockMinedEvent = new BlockMinedEvent(currentBlock.Index);
        var msg = OutboxMessage.FromDomainEvent(blockMinedEvent, dateTimeProvider);
        dbContext.Set<OutboxMessage>().Add(msg);

        await dbContext.SaveChangesAsync(ct);

        return currentBlock;
    }
}