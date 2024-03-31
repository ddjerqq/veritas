namespace Application.Common.Abstractions;

public interface IProcessedVotesCache
{
    public bool Contains(string hash);

    public void Add(string hash);
}