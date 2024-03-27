using Application.Abstractions;
using Application.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Common;
using Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services;

public sealed class InMemoryProcessedVotesCache : IProcessedVotesCache
{
    public List<Vote> Votes { get; init; } = [];

    public Vote? GetByHash(string hash) => Votes.FirstOrDefault(vote => vote.Hash.ToHexString() == hash);

    public void Set(Vote vote) => Votes.Add(vote);
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryProcessedVoteCache(this IServiceCollection services)
    {
        services.AddSingleton<IProcessedVotesCache, InMemoryProcessedVotesCache>(sp =>
        {
            using var scope = sp.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

            var votes = dbContext
                .Set<VoteDto>()
                .ProjectTo<Vote>(mapper.ConfigurationProvider)
                .ToList();

            return new InMemoryProcessedVotesCache
            {
                Votes = votes,
            };
        });

        return services;
    }
}