using Domain.Entities;

namespace Application.Common.Abstractions;

// TODO client implement this on the client side as well
public interface ICurrentVoterAccessor
{
    public Voter? GetCurrentVoter();
}