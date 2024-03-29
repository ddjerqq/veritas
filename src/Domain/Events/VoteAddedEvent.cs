using Domain.Abstractions;

namespace Domain.Events;

public sealed record VoteAddedEvent(string Hash) : IDomainEvent;
