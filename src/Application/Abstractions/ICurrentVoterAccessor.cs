using Domain.ValueObjects;

namespace Application.Abstractions;

public interface ICurrentVoterAccessor
{
    public Voter? GetCurrentVoter(CancellationToken ct = default);
}