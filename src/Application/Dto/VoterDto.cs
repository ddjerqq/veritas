using Domain.Common;
using Domain.ValueObjects;

namespace Application.Dto;

public sealed record VoterDto(string Address, string PublicKey, IEnumerable<VoteDto> Votes)
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

    public static VoterDto RandomVoterDto(int voteCount)
    {
        var addr = "0x" + StringExt.RandomHexString(42);

        var votes = Enumerable.Range(0, voteCount)
            .Select(_ => VoteDto.RandomVoteDto())
            .ToList();

        return new VoterDto(addr, StringExt.RandomHexString(128), votes);
    }
}