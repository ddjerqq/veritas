using Application.Common.Abstractions;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Blockchain.Queries;

public record GetPartyVotes : IRequest<Dictionary<Party, int>>;

// ReSharper disable once UnusedType.Global
internal sealed class GetPartyVotesQueryHandler(IAppDbContext dbContext) : IRequestHandler<GetPartyVotes, Dictionary<Party, int>>
{
    public async Task<Dictionary<Party, int>> Handle(GetPartyVotes request, CancellationToken ct)
    {
        var parties = await dbContext.Set<Vote>()
            .AsNoTracking()
            .GroupBy(v => v.PartyId)
            .Select(g => new
            {
                Party = g.Key,
                Count = g.Count(),
            })
            .ToListAsync(ct);

        return parties.ToDictionary(kv => (Party)kv.Party, kv => kv.Count);
    }
}