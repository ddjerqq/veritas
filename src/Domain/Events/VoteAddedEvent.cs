using Domain.Abstractions;

namespace Domain.Events;

public sealed record VoteAddedEvent(Guid ItemId) : IDomainEvent;