﻿using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Aggregates;

public record Block
{
    public const int Difficulty = 6;
    public const int VoteLimit = 256;

    public long Index { get; private init; }

    public long Nonce { get; private init; }

    public byte[] Hash => SHA256.HashData(HashPayload);

    public byte[] MerkleRoot => Common.MerkleRoot.BuildMerkleRoot(Votes.Select(v => v.Hash));

    public byte[] PreviousHash { get; private init; } = default!;

    public List<Vote> Votes { get; } = [];

    public bool IsHashValid => Hash.ToHexString().StartsWith(new string('0', Difficulty));

    [Pure]
    public Block Mine()
    {
        if (IsHashValid) return this;

        // var coreCount = Environment.ProcessorCount;
        var coreCount = 1;
        long foundNonce = 0;
        byte[] pred = new string('0', Difficulty).ToBytesFromHex();

        Parallel.For(0, coreCount, (i, state) =>
        {
            long nonce = i;
            byte[] payload = HashPayload;
            int destOffset = payload.Length - sizeof(long);

            byte[] nonceBuffer;

            while (!state.IsStopped)
            {
                nonceBuffer = BitConverter.GetBytes(nonce);
                Buffer.BlockCopy(nonceBuffer, 0, payload, destOffset, sizeof(long));

                // TODO optimize use spans.
                byte[] hash = SHA256.HashData(payload);

                // hash converted to hex, must equal Difficulty
                if (pred.ArrayEquals(hash[..(Difficulty / 2)]))
                {
                    foundNonce = nonce;
                    state.Stop();
                }

                nonce += coreCount;
            }
        });

        return this with { Nonce = foundNonce };
    }

    [Pure]
    public Block Next()
    {
        return this with
        {
            Index = Index + 1,
            Nonce = 0,
            PreviousHash = Hash,
        };
    }

    public static Block Genesis()
    {
        var block = new Block
        {
            Index = 0,
            Nonce = 14261917,
            PreviousHash = new byte[32],
        };

        return block;
    }

    private byte[] HashPayload
    {
        get
        {
            var buffer = new List<byte>();
            buffer.AddRange(BitConverter.GetBytes(Index));
            buffer.AddRange(PreviousHash);
            buffer.AddRange(MerkleRoot);
            buffer.AddRange(BitConverter.GetBytes(Nonce));
            return buffer.ToArray();
        }
    }
}

// public static explicit operator Block(BlockDto dto)
// {
//     var block = new Block
//     {
//         Index = dto.Index,
//         Nonce = dto.Nonce,
//         PreviousHash = dto.PreviousHash.ToBytesFromHex(),
//     };
//     block._votes.AddRange(dto.Votes.Select(x => (Vote)x));
//
//     if (!block.IsHashValid || block.Hash.ToHexString() != dto.Hash)
//         throw new InvalidOperationException("Invalid block hash");
//
//     if (block.MerkleRoot.ToHexString() != dto.MerkleRoot)
//         throw new InvalidOperationException("Invalid merkle root");
//
//     return block;
// }
//
// public static explicit operator BlockDto(Block block)
// {
//     return new BlockDto
//     {
//         Index = block.Index,
//         Nonce = block.Nonce,
//         Hash = block.Hash.ToHexString(),
//         MerkleRoot = block.MerkleRoot.ToHexString(),
//         PreviousHash = block.PreviousHash.ToHexString(),
//         Votes = block.Votes.Select(x => (VoteDto)x).ToList(),
//     };
// }
