using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Abstractions;
using Domain.Abstractions;

namespace Application.Common;

public sealed class OutboxMessage
{
    private static readonly JsonSerializerOptions JsonSerializerSettings = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        AllowTrailingCommas = true,
    };

    public Guid Id { get; init; } = Guid.NewGuid();

    public string Type { get; init; } = default!;

    public string Content { get; init; } = string.Empty;

    public DateTime OccuredOnUtc { get; init; }

    public DateTime? ProcessedOnUtc { get; set; }

    public string? Error { get; set; }

    public static OutboxMessage FromDomainEvent(IDomainEvent domainEvent, IDateTimeProvider dateTimeProvider)
    {
        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = domainEvent.GetType().Name,
            Content = JsonSerializer.Serialize(domainEvent, JsonSerializerSettings),
            OccuredOnUtc = dateTimeProvider.UtcNow,
            ProcessedOnUtc = null,
            Error = null,
        };
    }
}