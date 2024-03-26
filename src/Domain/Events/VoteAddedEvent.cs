using Domain.Abstractions;
using Domain.ValueObjects;

namespace Domain.Events;

public sealed record VoteAddedEvent(Vote Vote) : IDomainEvent;