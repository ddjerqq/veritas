using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using Domain.Common;
using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Aggregates;

public record Block
{
    private const int Difficulty = 6;

    /// <summary>
    /// The number of votes allowed in one block, the less this number, the more fault-tolerant the app will be,
    /// because we do not store votes that have not been added to the blockchain yet.
    /// </summary>
    public const int VoteLimit = 128;

    public long Index { get; init; }

    public long Nonce { get; init; }

    public byte[] PreviousHash { get; init; } = new byte[32];

    public List<Vote> Votes { get; init; } = [];

    public byte[] MerkleRoot => Common.MerkleRoot.BuildMerkleRoot(Votes.Select(v => v.Hash).ToList());

    public byte[] Hash => SHA256.HashData(HashPayload);

    public bool IsHashValid => Hash.ToHexString().StartsWith(new string('0', Difficulty));

    public bool TryAddVote(Vote vote)
    {
        if (Votes.Count >= VoteLimit || !vote.IsHashValid || !vote.IsSignatureValid)
            return false;

        vote.BlockIndex = Index;
        Votes.Add(vote);

        return true;
    }

    [Pure]
    public Block Mine()
    {
        if (IsHashValid) return this;
        var foundNonce = Miner.Mine(HashPayload, Difficulty);
        return this with { Nonce = foundNonce };
    }

    [Pure]
    public Block Next()
    {
        return this with
        {
            Index = Index + 1,
            Nonce = 0,
            Votes = [],
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
