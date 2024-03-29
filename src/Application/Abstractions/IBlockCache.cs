using Application.Dtos;

namespace Application.Abstractions;

public interface IBlockCache
{
    protected BlockDto? Current { get; set; }

    public Task<BlockDto> GetCurrentAsync(CancellationToken ct = default);

    public void SetCurrent(BlockDto block) => Current = block;
}