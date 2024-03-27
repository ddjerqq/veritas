using Domain.ValueObjects;

namespace Application.Abstractions;

// TODO implement this on the client side as well
public interface ICurrentVoterAccessor
{
    public Voter? GetCurrentVoter();
}