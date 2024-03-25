using Application.Abstractions;
using Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public sealed class ProcessVotesBackgroundJob(
    AppDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    ILogger<ProcessVotesBackgroundJob> logger) : IJob
{
    public static readonly JobKey Key = new("process_votes");

    public async Task Execute(IJobExecutionContext context)
    {
        // await foreach (var vote in voteQueue.DequeueAllAsync(context.CancellationToken))
        // {
        //     if (CurrentBlock.Votes.Count == Block.VoteLimit)
        //     {
        //         // mine first, and rotate the block
        //         var minedBlock = CurrentBlock.Mine();
        //         var next = CurrentBlock.Next();
        //     }
        //
        //     var voteDto = (VoteDto)vote;
        //     voteDto.BlockIndex = CurrentBlock.Index;
        //     dbContext.Votes.Add(voteDto);
        //
        //     await dbContext.SaveChangesAsync(context.CancellationToken);
        // }
    }
}