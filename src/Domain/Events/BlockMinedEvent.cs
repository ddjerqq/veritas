using Domain.Abstractions;

namespace Domain.Events;

public sealed record BlockMinedEvent(Guid ItemId) : IDomainEvent;