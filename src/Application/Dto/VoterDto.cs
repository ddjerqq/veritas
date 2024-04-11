using System.Security.Cryptography;
using Application.ValueObjects;
using Domain.Common;

namespace Application.Dto;

public record VoterDto(string Address, List<VoteDto> Votes)
{
    public string ShortAddress => Address[..16];

    public Party? FavoriteParty => Votes
        .GroupBy(vote => vote.Party.Id)
        .Select(group => new
        {
            Party = group.Key,
            Count = group.Count(),
        })
        .MaxBy(partyCounts => partyCounts.Count)?
        .Party;

    public DateTime? LastVoteTime => Votes.MaxBy(vote => vote.Added)?.Added;

    public static VoterDto RandomVoterDto(int voteCount)
    {
        var addr = "0x" + RandomString()[22..];

        var votes = Enumerable.Range(0, voteCount)
            .Select(_ => VoteDto.RandomVoteDto(addr))
            .ToList();

        return new VoterDto(addr, votes);

        static string RandomString()
        {
            var buffer = new byte[16];
            Random.Shared.NextBytes(buffer);
            return SHA256.HashData(buffer).ToHexString();
        }
    }
}