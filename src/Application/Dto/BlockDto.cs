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

    public long Index { get; }

    public long Nonce { get; }

    public string Hash { get; } = default!;

    public string MerkleRoot { get; } = default!;

    public string PreviousHash { get; } = default!;

    public List<VoteDto> Votes { get; } = [];

    public string ShortHash => $"{Hash[..8]}-{Hash[^8..]}";

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

    public static BlockDto RandomBlockDto(long? index = null, int? votes = null)
    {
        return new BlockDto(
            index ?? Random.Shared.Next(0, 10_000_000),
            Random.Shared.NextInt64(0, 10_000_000),
            StringExt.RandomHexString(64),
            StringExt.RandomHexString(64),
            StringExt.RandomHexString(64),
            Enumerable.Range(0, Random.Shared.Next(1, votes ?? 100))
                .Select(_ => VoteDto.RandomVoteDto())
                .ToList());
    }
}