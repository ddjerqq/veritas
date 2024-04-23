using Domain.ValueObjects;

namespace Application.Dto;

public sealed record VoterDto(string Address, string PublicKey, List<VoteDto> Votes)
{
    public string ShortAddress => Address[..8];

    public Party? FavoriteParty => Votes
        .GroupBy(vote => vote.PartyId)
        .Select(group => new
        {
            Party = group.Key,
            Count = group.Count(),
        })
        .MaxBy(partyCounts => partyCounts.Count)?
        .Party;

    public DateTime? LastVoteTime => Votes.MaxBy(vote => vote.Timestamp)?.Timestamp;
}