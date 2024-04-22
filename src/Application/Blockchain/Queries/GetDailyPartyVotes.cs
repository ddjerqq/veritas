using Application.Common.Abstractions;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Blockchain.Queries;

public record GetDailyPartyVotes(int Days = 15) : IRequest<Dictionary<int, Dictionary<DateOnly, int>>>;

// ReSharper disable once UnusedType.Global
internal sealed class GetDailyPartyVotesQueryHandler(IAppDbContext dbContext, IDateTimeProvider dateTimeProvider)
    : IRequestHandler<GetDailyPartyVotes, Dictionary<int, Dictionary<DateOnly, int>>>
{
    public async Task<Dictionary<int, Dictionary<DateOnly, int>>> Handle(GetDailyPartyVotes request, CancellationToken ct)
    {
        var now = dateTimeProvider.UtcNow;
        var after = now.Date.AddDays(-request.Days);

        var votesInTheLastNDays = await dbContext.Set<Vote>()
            .AsNoTracking()
            .Where(v => v.Timestamp.Date >= after)
            .ToListAsync(ct);

        var dailyVoteCounts = votesInTheLastNDays
            .GroupBy(v => v.PartyId)
            .Select(group => new
            {
                Party = (Party)group.Key,
                VotesThroughoutDays = group
                    .Where(v => v.PartyId == group.Key)
                    .GroupBy(v => (now - v.Timestamp).Days)
                    .ToDictionary(
                        subGroup => DateOnly.FromDateTime(now.Date.AddDays(-subGroup.Key)),
                        subGroup => subGroup.Count()),
            })
            .ToDictionary(kv => (int)kv.Party, kv => kv.VotesThroughoutDays);

        return dailyVoteCounts;
    }
}