using System.Diagnostics.Contracts;
using Domain.Entities;

namespace Application.Common.Abstractions;

// TODO client implement this on the client side as well
public interface ICurrentVoterAccessor
{
    [Pure]
    public Voter? TryGetCurrentVoter();

    [Pure]
    public Voter GetCurrentVoter()
    {
        return TryGetCurrentVoter() ?? throw new InvalidOperationException("No voter is currently logged in");
    }
}