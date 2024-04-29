using AutoMapper;
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

    public long Index { get; }

    public long Nonce { get; }

    public string Hash { get; } = default!;

    public string MerkleRoot { get; } = default!;

    public string PreviousHash { get; } = default!;

    public List<VoteDto> Votes { get; } = [];

    public string ShortHash => $"{Hash[..4]}-{Hash[^4..]}";

    public string ShortPreviousHash => $"{PreviousHash[..4]}-{PreviousHash[^4..]}";

    public string ShortMerkleRoot => $"{MerkleRoot[..4]}-{MerkleRoot[^4..]}";

    public int SizeBytes => 128 + Votes.Count * 208;

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