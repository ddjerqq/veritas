using Api.Data.Models;

namespace Api.Services.Interfaces;

public interface IVoteService
{
    public Task AddVoteAsync(Vote vote, CancellationToken ct);
}