using Domain.Abstractions;

namespace Domain.Events;

public sealed record BlockMinedEvent(long BlockIndex) : IDomainEvent;