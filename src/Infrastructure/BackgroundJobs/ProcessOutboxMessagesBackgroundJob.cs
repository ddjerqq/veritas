using Application.Abstractions;
using Application.Common;
using Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;

namespace Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public sealed class ProcessOutboxMessagesBackgroundJob(
    IPublisher publisher,
    IAppDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    ILogger<ProcessOutboxMessagesBackgroundJob> logger)
    : IJob
{
    public static readonly JobKey Key = new("process_outbox_messages");
    private const int MessagesPerBatch = 20;

    public async Task Execute(IJobExecutionContext context)
    {
        var messages = await dbContext
            .Set<OutboxMessage>()
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccuredOnUtc)
            .Take(MessagesPerBatch)
            .ToListAsync(context.CancellationToken);

        foreach (var message in messages)
        {
            var domainEvent = JsonConvert
                .DeserializeObject<IDomainEvent>(message.Content, OutboxMessage.JsonSerializerSettings);

            if (domainEvent is null)
            {
                logger.LogWarning("Failed to deserialize message {MessageId}", message.Id);
                continue;
            }

            try
            {
                await publisher.Publish(domainEvent, context.CancellationToken);
            }
            catch (Exception ex)
            {
                message.Error = ex.ToString();
                logger.LogError(ex, "Failed to publish message {MessageId}", message.Id);
            }

            message.ProcessedOnUtc = dateTimeProvider.UtcNow;
        }

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}