using Domain.Common;

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
    public int SizeMegaBytes => Votes.Count * 208 / 1024;

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