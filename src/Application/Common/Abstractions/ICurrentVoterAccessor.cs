using Domain.Entities;

namespace Application.Common.Abstractions;

public interface ICurrentVoterAccessor
{
    public Voter? TryGetCurrentVoter();

    public Voter GetCurrentVoter() =>
        TryGetCurrentVoter()
        ?? throw new InvalidOperationException("Tried accessing the Current voter but it was null.");
}