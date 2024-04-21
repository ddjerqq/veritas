using Application.Common.Abstractions;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Blockchain.Queries;

public record GetDailyPartyVotes(int Days = 15) : IRequest<Dictionary<Party, Dictionary<DateOnly, int>>>;

// ReSharper disable once UnusedType.Global
internal sealed class GetDailyPartyVotesQueryHandler(IAppDbContext dbContext, IDateTimeProvider dateTimeProvider)
    : IRequestHandler<GetDailyPartyVotes, Dictionary<Party, Dictionary<DateOnly, int>>>
{
    public async Task<Dictionary<Party, Dictionary<DateOnly, int>>> Handle(GetDailyPartyVotes request, CancellationToken ct)
    {
        var now = dateTimeProvider.UtcNow;
        var nowDate = DateOnly.FromDateTime(now);

        var votesInTheLastNDays = await dbContext.Set<Vote>()
            .AsNoTracking()
            .Where(v => (now - v.Timestamp).Days <= request.Days)
            .GroupBy(v => v.PartyId)
            .Select(group => new
            {
                Party = (Party)group.Key,
                VotesThroughoutDays = group
                    .Where(v => v.PartyId == group.Key)
                    .GroupBy(v => (now - v.Timestamp).Days)
                    .ToDictionary(
                        subGroup => nowDate.AddDays(-subGroup.Key),
                        subGroup => subGroup.Count()),
            })
            .ToListAsync(ct);

        return votesInTheLastNDays.ToDictionary(kv => kv.Party, kv => kv.VotesThroughoutDays);
    }
}