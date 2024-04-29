using Application.Common.Abstractions;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services;

// TODO replace with REDIS implementation, so its distributed
public sealed class InMemoryProcessedVotesCache(HashSet<string> hashes) : IProcessedVotesCache
{
    public bool Contains(string hash)
    {
        return hashes.Contains(hash);
    }

    public void Add(string hash)
    {
        hashes.Add(hash);
    }
}

public static class InMemoryProcessedVotesCacheExt
{
    public static void AddInMemoryProcessedVoteCacheSingleton(this IServiceCollection services)
    {
        services.AddSingleton<IProcessedVotesCache, InMemoryProcessedVotesCache>(sp =>
        {
            using var scope = sp.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

            var voteHashes = dbContext
                .Set<Vote>()
                .Select(x => x.Hash)
                .ToHashSet();

            return new InMemoryProcessedVotesCache(voteHashes);
        });
    }
}