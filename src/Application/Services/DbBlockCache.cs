using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public sealed class DbBlockCache(IAppDbContext dbContext) : IBlockCache
{
    public Block? Current { get; set; }

    public async Task<Block> GetCurrentAsync(CancellationToken ct = default)
    {
        if (Current is not null)
            return Current;

        Current = await dbContext
            .Set<Block>()
            .Include(x => x.Votes)
            .OrderBy(x => x.Index)
            .LastAsync(ct);

        return Current;
    }
}