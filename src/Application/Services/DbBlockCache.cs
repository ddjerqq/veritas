using Application.Abstractions;
using Application.Dtos;
using Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public sealed class DbBlockCache(IAppDbContext dbContext) : IBlockCache
{
    private Block? _current;

    public async Task<Block> GetCurrentAsync(CancellationToken ct = default)
    {
        if (_current is { } current)
            return current;

        var lastDto = await dbContext.Set<BlockDto>()
            .OrderBy(x => x.Index)
            .LastAsync(ct);

        _current = (Block)lastDto;

        return _current;
    }

    public async Task<Block> MineAndRotateAsync(CancellationToken ct = default)
    {
        // TODO add logging.
        var block = await GetCurrentAsync(ct);
        var minedBlock = block.Mine();
        var next = minedBlock.Next();

        _current = next;

        dbContext.Set<BlockDto>().Update(minedBlock);
        dbContext.Set<BlockDto>().Add(next);

        return minedBlock;
    }
}