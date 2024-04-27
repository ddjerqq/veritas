using System.Diagnostics.Contracts;
using Domain.Entities;

namespace Application.Common.Abstractions;

public interface ICurrentVoterAccessor
{
    [Pure]
    public Voter? TryGetCurrentVoter();
}