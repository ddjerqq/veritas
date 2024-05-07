using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Dto;

[AutoMap(typeof(Block), ReverseMap = true)]
public record BlockDto()
{
    public BlockDto(long index, long nonce, string hash, string merkleRoot, string previousHash, IEnumerable<VoteDto> votes) : this()
    {
        Index = index;
        Nonce = nonce;
        Hash = hash;
        MerkleRoot = merkleRoot;
        PreviousHash = previousHash;
        Votes = votes.ToList();
    }

    public long Index { get; init; }

    public long Nonce { get; init; }

    public string Hash { get; init; } = default!;

    public string MerkleRoot { get; init; } = default!;

    public string PreviousHash { get; init; } = default!;

    public List<VoteDto> Votes { get; init; } = [];

    public string ShortHash => $"{Hash?[..8]}-{Hash?[^8..]}";

    public string ShortPreviousHash => $"{PreviousHash?[..4]}-{PreviousHash?[^4..]}";

    public string ShortMerkleRoot => $"{MerkleRoot?[..4]}-{MerkleRoot?[^4..]}";

    public DateTime Mined => Votes?
        .MaxBy(v => v.Timestamp)?
        .Timestamp ?? Block.GenesisDate;

    public Party? TopParty => Votes?
        .GroupBy(vote => vote.PartyId)
        .Select(group => new
        {
            Party = group.Key,
            Count = group.Count(),
        })
        .MaxBy(partyCounts => partyCounts.Count)?
        .Party;
}