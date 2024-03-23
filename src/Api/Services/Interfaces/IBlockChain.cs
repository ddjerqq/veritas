using Api.Data.Models;

namespace Api.Services.Interfaces;

public interface IBlockChain
{
    public Block CurrentBlock { get; }

    public IReadOnlyCollection<Block> Blocks { get; }

    public Task<bool> TryAddVoteAsync(Vote vote, CancellationToken ct = default);

    public Task<Block?> GetByIndexAsync(long blockIndex, CancellationToken ct = default);

    public Task<Block?> GetByHashAsync(string blockHash, CancellationToken ct = default);
}