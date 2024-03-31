using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Dtos;

public record BlockDto
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Index { get; private set; }

    public long Nonce { get; private set; }

    public string Hash { get; private set; } = default!;

    public string MerkleRoot { get; private set; } = default!;

    public string PreviousHash { get; private set; } = default!;

    public ICollection<VoteDto> Votes { get; private set; } = [];

    public void CopyFrom(Block block)
    {
        Index = block.Index;
        Nonce = block.Nonce;
        Hash = block.Hash.ToHexString();
        MerkleRoot = block.MerkleRoot.ToHexString();
        PreviousHash = block.PreviousHash.ToHexString();
        Votes = block.Votes.Select(v => (VoteDto)v).ToList();
    }

    public static explicit operator Block(BlockDto source)
    {
        var block = new Block
        {
            Index = source.Index,
            Nonce = source.Nonce,
            PreviousHash = source.PreviousHash.ToBytesFromHex(),
            Votes = source.Votes.Select(v => (Vote)v).ToList(),
        };

        // this is disabled, because we can have incomplete blocks stored in the database
        // if (!block.IsHashValid)
        //     throw new InvalidOperationException($"failed to convert Block, invalid hash: {block.Hash.ToHexString()}");

        if (source.Hash != block.Hash.ToHexString())
            throw new InvalidOperationException($"failed to convert Block, Invalid hash, expected: {block.Hash.ToHexString()} but was: {source.Hash}");

        if (source.MerkleRoot != block.MerkleRoot.ToHexString())
            throw new InvalidOperationException(
                $"failed to convert Block, Invalid merkle root, expected: {block.MerkleRoot.ToHexString()} but was: {source.MerkleRoot}");

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
