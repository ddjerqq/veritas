using Domain.Common;
using Domain.ValueObjects;

namespace Application.Dto;

public record BlockDto(
    long Index,
    long Nonce,
    string Hash,
    string MerkleRoot,
    string PreviousHash,
    List<VoteDto> Votes,
    DateTime Mined)
{
    public string ShortHash => $"{Hash[..4]}-{Hash[^4..]}";

    public string ShortPreviousHash => $"{PreviousHash[..4]}-{PreviousHash[^4..]}";

    public string ShortMerkleRoot => $"{MerkleRoot[..4]}-{MerkleRoot[^4..]}";

    public int SizeMegaBytes => Votes.Count * 208 / 1024;

    public Party TopParty => Votes
        .GroupBy(vote => vote.Party.Id)
        .Select(group => new
        {
            Party = group.Key,
            Count = group.Count(),
        })
        .MaxBy(partyCounts => partyCounts.Count)!
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
                .ToList(),
            DateTime.Now);
    }
}