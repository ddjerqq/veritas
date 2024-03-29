using Domain.Aggregates;

namespace Application.Abstractions;

public interface IBlockCache
{
    public Task<Block> GetCurrentAsync(CancellationToken ct = default);

    /// <summary>
    /// This method will mine the current block and return the mined block.
    /// It will set its predecessor to the current block.
    /// </summary>
    /// <note>
    /// It is the caller's responsibility to add the mined block to the database
    /// </note>
    public Task<Block> MineAndRotateAsync(CancellationToken ct = default);
}