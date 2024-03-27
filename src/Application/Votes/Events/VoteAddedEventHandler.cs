using Application.Abstractions;
using Application.Dtos;
using AutoMapper;
using Domain.Aggregates;
using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Votes.Events;

public class VoteAddedEventHandler(
    ILogger<VoteAddedEventHandler> logger,
    IMapper mapper,
    IAppDbContext dbContext,
    IBlockCache blockCache) : INotificationHandler<VoteAddedEvent>
{
    public async Task Handle(VoteAddedEvent notification, CancellationToken ct)
    {
        var currentBlock = blockCache.GetCurrent();

        // if we are due to mining
        if (currentBlock.Votes.Count >= Block.VoteLimit)
        {
            currentBlock = blockCache.MineAndRotate();
            var minedBlockDto = mapper.Map<Block, BlockDto>(currentBlock);
            dbContext.Set<BlockDto>().Add(minedBlockDto);
            await dbContext.SaveChangesAsync(ct);

            // TODO add BlockMinedEvent to outbox
        }

        var vote = notification.GetVote();

        // TODO we will need a global vote cache.
        // preferably on the database. maybe we could store them in the databse?
        // and the blocks in the database will be eventually consistent?
        // each vote addition will be one write to the database

        // make a table, like VoteQueue, then we will process them one by one.
        currentBlock.TryAddVote(vote);

        logger.LogInformation("{Address} voted for party {PartyId} at {Timestamp}", vote.Voter.Address, vote.PartyId, vote.Timestamp);
    }
}