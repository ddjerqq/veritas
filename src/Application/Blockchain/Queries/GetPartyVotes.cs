using Application.Common.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Blockchain.Queries;

public record GetPartyVotes : IRequest<Dictionary<int, int>>;

// ReSharper disable once UnusedType.Global
internal sealed class GetPartyVotesQueryHandler(IAppDbContext dbContext) : IRequestHandler<GetPartyVotes, Dictionary<int, int>>
{
    public async Task<Dictionary<int, int>> Handle(GetPartyVotes request, CancellationToken ct)
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

        return parties.ToDictionary(kv => kv.Party, kv => kv.Count);
    }
}