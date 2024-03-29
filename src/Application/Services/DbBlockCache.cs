using Application.Abstractions;
using Application.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public sealed class DbBlockCache(IAppDbContext dbContext, IMapper mapper) : IBlockCache
{
    private Block? _current;

    public async Task<Block> GetCurrentAsync(CancellationToken ct = default)
    {
        if (_current is { } current)
            return current;

        _current = await dbContext.Set<BlockDto>()
            .OrderBy(x => x.Index)
            .ProjectTo<Block>(mapper.ConfigurationProvider)
            .LastAsync(ct);

        return _current;
    }

    public async Task<Block> MineAndRotateAsync(CancellationToken ct = default)
    {
        // TODO add logging.
        var block = await GetCurrentAsync(ct);
        var minedBlock = block.Mine();
        var next = minedBlock.Next();

        _current = next;

        var minedBlockDto = mapper.Map<Block, BlockDto>(minedBlock);
        dbContext.Set<BlockDto>().Update(minedBlockDto);

        var nextDto = mapper.Map<Block, BlockDto>(next);
        dbContext.Set<BlockDto>().Add(nextDto);

        return minedBlock;
    }
}