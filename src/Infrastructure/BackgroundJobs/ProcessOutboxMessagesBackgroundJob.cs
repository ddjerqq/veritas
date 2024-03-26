using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Abstractions;
using Application.Common;
using Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public class ProcessOutboxMessagesBackgroundJob(
    IPublisher publisher,
    IAppDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    ILogger<ProcessOutboxMessagesBackgroundJob> logger)
    : IJob
{
    public static readonly JobKey Key = new("process_outbox_messages");

    private static readonly JsonSerializerOptions JsonSerializerSettings = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        AllowTrailingCommas = true,
    };

    public async Task Execute(IJobExecutionContext context)
    {
        var messages = await dbContext
            .Set<OutboxMessage>()
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccuredOnUtc)
            .Take(20)
            .ToListAsync(context.CancellationToken);

        foreach (var message in messages)
        {
            var domainEvent = JsonSerializer
                .Deserialize<IDomainEvent>(message.Content, JsonSerializerSettings);

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