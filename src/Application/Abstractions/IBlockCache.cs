using Domain.Entities;

namespace Application.Abstractions;

public interface IBlockCache
{
    protected Block? Current { get; set; }

    public Task<Block> GetCurrentAsync(CancellationToken ct = default);

    public void SetCurrent(Block block) => Current = block;
}