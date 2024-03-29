using Domain.Aggregates;
using Domain.Common;
using Domain.ValueObjects;

namespace Application.Dtos;

public record BlockDto
{
    public long Index { get; init; }

    public long Nonce { get; init; }

    public string Hash { get; init; } = default!;

    public string MerkleRoot { get; init; } = default!;

    public string PreviousHash { get; init; } = default!;

    public ICollection<VoteDto> Votes { get; init; } = [];

    public static explicit operator Block(BlockDto source)
    {
        var block = new Block
        {
            Index = source.Index,
            Nonce = source.Nonce,
            PreviousHash = source.PreviousHash.ToBytesFromHex(),
            Votes = source.Votes.Select(v => (Vote)v).ToList(),
        };

        if (!block.IsHashValid)
            throw new InvalidOperationException($"failed to convert Block, invalid hash: {block.Hash.ToHexString()}");

        if (source.Hash != block.Hash.ToHexString())
            throw new InvalidOperationException($"failed to convert Block, expected: {source.Hash} was: {block.Hash.ToHexString()}");

        if (source.MerkleRoot != block.MerkleRoot.ToHexString())
            throw new InvalidOperationException(
                $"failed to convert Block, expected: {source.MerkleRoot} was: {block.MerkleRoot.ToHexString()}");

        return block;
    }

    public static implicit operator BlockDto(Block source)
    {
        return new BlockDto
        {
            Index = source.Index,
            Nonce = source.Nonce,
            Hash = source.Hash.ToHexString(),
            MerkleRoot = source.MerkleRoot.ToHexString(),
            PreviousHash = source.PreviousHash.ToHexString(),
            Votes = source.Votes.Select(v => (VoteDto)v).ToList(),
        };
    }
}
