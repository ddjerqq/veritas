using Application.Abstractions;
using Application.Dtos;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services;

public sealed class InMemoryProcessedVotesCache(HashSet<string> hashes) : IProcessedVotesCache
{
    public bool Contains(string hash) => hashes.Contains(hash);

    public void Add(string hash) => hashes.Add(hash);
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryProcessedVoteCache(this IServiceCollection services)
    {
        services.AddSingleton<IProcessedVotesCache, InMemoryProcessedVotesCache>(sp =>
        {
            using var scope = sp.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

            var voteHashes = dbContext
                .Set<VoteDto>()
                .Select(x => x.Hash)
                .ToHashSet();

            return new InMemoryProcessedVotesCache(voteHashes);
        });

        return services;
    }
}