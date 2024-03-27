using Domain.Abstractions;
using Domain.Aggregates;

namespace Domain.Events;

public sealed record BlockMinedEvent(long BlockIndex) : IDomainEvent;