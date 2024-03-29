namespace Application.Abstractions;

public interface IProcessedVotesCache
{
    public bool Contains(string hash);

    public void Add(string hash);
}