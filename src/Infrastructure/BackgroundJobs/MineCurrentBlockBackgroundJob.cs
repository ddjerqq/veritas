using Application.Blockchain.Queries;
using Application.Common.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public sealed class MineCurrentBlockBackgroundJob(IAppDbContext dbContext, IMediator mediator, ILogger<MineCurrentBlockBackgroundJob> logger) : IJob
{
    public static readonly JobKey Key = new("mine_current_block");

    public async Task Execute(IJobExecutionContext context)
    {
        // fetch all transactions without a blockIndex
        var votes = await dbContext.Set<Vote>()
            .Include(v => v.Voter)
            .Where(v => v.BlockIndex == null)
            .ToListAsync(context.CancellationToken);

        if (votes.Count == 0)
        {
            logger.LogInformation("A block was not mined, because there were no new votes");
            return;
        }

        // compile them into a block
        var lastBlockQuery = new GetLastBlockQuery();
        var lastBlock = await mediator.Send(lastBlockQuery, context.CancellationToken);
        var block = lastBlock.NextBlock();

        block.AddVotes(votes);

        // mine the block.
        await Task.Run(() => block.Mine());

        // insert it into the database.
        dbContext.Set<Block>().Add(block);
        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}