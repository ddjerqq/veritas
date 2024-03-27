using System.ComponentModel.DataAnnotations;
using Application.Abstractions;
using Domain.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Application.Common;


// TODO optimization add prop Processed, with index. so we dont slow down the loading
public sealed class OutboxMessage
{
    public static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        Converters =
        {
            new StringEnumConverter(),
            new ByteArrayJsonConverter(),
        },
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy(),
        },
    };

    public Guid Id { get; init; } = Guid.NewGuid();

    [StringLength(128)]
    public string Type { get; init; } = string.Empty;

    [StringLength(2048)]
    public string Content { get; init; } = string.Empty;

    public DateTime OccuredOnUtc { get; init; }

    public DateTime? ProcessedOnUtc { get; set; }

    [StringLength(4096)]
    public string? Error { get; set; }

    public static OutboxMessage FromDomainEvent(IDomainEvent domainEvent, IDateTimeProvider dateTimeProvider)
    {
        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = domainEvent.GetType().Name,
            Content = JsonConvert.SerializeObject(domainEvent, JsonSerializerSettings),
            OccuredOnUtc = dateTimeProvider.UtcNow,
            ProcessedOnUtc = null,
            Error = null,
        };
    }
}