using Domain.Abstractions;

namespace Domain.Events;

public sealed record UserVotedEvent(Guid ItemId) : IDomainEvent;