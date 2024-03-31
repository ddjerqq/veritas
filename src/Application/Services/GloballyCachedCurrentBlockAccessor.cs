using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services;

public sealed class GloballyCachedCurrentBlockAccessor(IServiceProvider sp) : ICurrentBlockAccessor
{
    public Block? Current { get; set; }

    public async Task<Block> GetCurrentBlockAsync(CancellationToken ct = default)
    {
        if (Current is not null)
            return Current;

        using var scope = sp.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        Current = await dbContext
            .Set<Block>()
            .AsNoTracking()
            .Include(x => x.Votes)
            .ThenInclude(x => x.Voter)
            .OrderBy(x => x.Index)
            .LastAsync(ct);

        return Current;
    }
}

public static class CurrentBlockAccessorExt
{
    public static void AddGloballyCachedCurrentBlockAccessorSingleton(this IServiceCollection services)
    {
        services.AddSingleton<ICurrentBlockAccessor, GloballyCachedCurrentBlockAccessor>();
    }
}