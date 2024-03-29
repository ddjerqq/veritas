using Application.Abstractions;
using Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public sealed class DbBlockCache(IAppDbContext dbContext) : IBlockCache
{
    public BlockDto? Current { get; set; }

    public async Task<BlockDto> GetCurrentAsync(CancellationToken ct = default)
    {
        if (Current is not null)
            return Current;

        Current = await dbContext
            .Set<BlockDto>()
            .Include(x => x.Votes)
            .OrderBy(x => x.Index)
            .LastAsync(ct);

        return Current;
    }
}