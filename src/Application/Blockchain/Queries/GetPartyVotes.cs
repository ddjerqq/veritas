using Application.Common.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Blockchain.Queries;

public record GetPartyVotes : IRequest<Dictionary<int, int>>;

// ReSharper disable once UnusedType.Global
internal sealed class GetPartyVotesQueryHandler(IMemoryCache cache, IAppDbContext dbContext) : IRequestHandler<GetPartyVotes, Dictionary<int, int>>
{
    public async Task<Dictionary<int, int>> Handle(GetPartyVotes request, CancellationToken ct)
    {
        if (cache.TryGetValue("party_vote_counts", out Dictionary<int, int>? data) && data is not null)
            return data;

        var dbData = await dbContext.Set<Vote>()
            .AsNoTracking()
            .GroupBy(v => v.PartyId)
            .Select(g => new
            {
                Party = g.Key,
                Count = g.Count(),
            })
            .ToListAsync(ct);

        data = dbData.ToDictionary(kv => kv.Party, kv => kv.Count);

        cache.Set("party_vote_counts", data, TimeSpan.FromMinutes(10));

        return data;
    }
}