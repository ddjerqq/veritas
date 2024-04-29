using AutoMapper;
using Domain.Entities;

namespace Application.Dto;

[AutoMap(typeof(Voter), ReverseMap = true)]
public sealed record VoterDto()
{
    public VoterDto(string address, string publicKey, List<VoteDto> votes) : this()
    {
        Address = address;
        PublicKey = publicKey;
        Votes = votes;
    }

    public string Address { get; init; } = default!;

    public string PublicKey { get; init; } = default!;

    public List<VoteDto> Votes { get; init; } = default!;

    public string ShortAddress => Address[..8];

    // public Party? FavoriteParty => Votes?
    //     .GroupBy(vote => vote.PartyId)
    //     .Select(group => new
    //     {
    //         Party = group.Key,
    //         Count = group.Count(),
    //     })?
    //     .MaxBy(partyCounts => partyCounts.Count)?
    //     .Party;

    public DateTime? LastVoteTime => Votes?.MaxBy(vote => vote.Timestamp)?.Timestamp;
}