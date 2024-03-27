using Domain.ValueObjects;

namespace Application.Abstractions;

public interface IProcessedVotesCache
{
    public Vote? GetByHash(string hash);

    public void Set(Vote vote);
}