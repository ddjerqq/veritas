using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using Api.Common;
using Api.Data.Dto;

namespace Api.Data.Models;

public record Block
{
    public const int Difficulty = 6;
    public const int VoteLimit = 256;
    private const int PayloadSize = sizeof(long) + sizeof(long) + 32 + 32;

    public long Index { get; private init; }

    public long Nonce { get; private init; }

    public byte[] Hash => SHA256.HashData(GetPayload());

    public byte[] MerkleRoot => Common.MerkleRoot.BuildMerkleRoot(Votes.Select(v => v.Hash));

    public byte[] PreviousHash { get; private init; } = default!;

    private readonly List<Vote> _votes = [];

    public IReadOnlyCollection<Vote> Votes => _votes.AsReadOnly();

    public bool IsHashValid => Hash.ToHexString().StartsWith(new string('0', Difficulty));

    [Pure]
    public bool TryAddVote(Vote vote)
    {
        if (!vote.VerifySignature(vote.Signature)) return false;
        vote.BlockIndex = Index;
        _votes.Add(vote);
        return true;
    }

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
            byte[] payload = GetPayload();
            byte[] nonceBuffer;

            while (!state.IsStopped)
            {
                nonceBuffer = BitConverter.GetBytes(nonce);
                Buffer.BlockCopy(nonceBuffer, 0, payload, PayloadSize - sizeof(long), sizeof(long));

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

    public static explicit operator Block(BlockDto dto)
    {
        var block = new Block
        {
            Index = dto.Index,
            Nonce = dto.Nonce,
            PreviousHash = dto.PreviousHash.ToBytesFromHex(),
        };
        block._votes.AddRange(dto.Votes.Select(x => (Vote)x));

        if (!block.IsHashValid || block.Hash.ToHexString() != dto.Hash)
            throw new InvalidOperationException("Invalid block hash");

        if (block.MerkleRoot.ToHexString() != dto.MerkleRoot)
            throw new InvalidOperationException("Invalid merkle root");

        return block;
    }

    public static explicit operator BlockDto(Block block)
    {
        return new BlockDto
        {
            Index = block.Index,
            Nonce = block.Nonce,
            Hash = block.Hash.ToHexString(),
            MerkleRoot = block.MerkleRoot.ToHexString(),
            PreviousHash = block.PreviousHash.ToHexString(),
            Votes = block.Votes.Select(x => (VoteDto)x).ToList(),
        };
    }

    private byte[] GetPayload()
    {
        var buffer = new byte[PayloadSize];
        var offset = 0;

        Buffer.BlockCopy(BitConverter.GetBytes(Index), 0, buffer, offset, sizeof(long));
        offset += sizeof(long);

        Buffer.BlockCopy(PreviousHash, 0, buffer, offset, 32);
        offset += 32;

        Buffer.BlockCopy(MerkleRoot, 0, buffer, offset, 32);
        offset += 32;

        Buffer.BlockCopy(BitConverter.GetBytes(Nonce), 0, buffer, offset, sizeof(long));

        return buffer;
    }
}