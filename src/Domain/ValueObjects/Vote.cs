using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using Domain.Common;

namespace Domain.ValueObjects;

public record Vote()
{
    private const int Difficulty = 6;

    public Voter Voter { get; init; } = default!;

    public int PartyId { get; init; }

    public long Timestamp { get; init; }

    public byte[] Signature { get; init; } = default!;

    public long Nonce { get; init; }

    public byte[] Hash => SHA256.HashData(HashPayload);

    public long BlockIndex { get; set; }  // for ef-core.

    public bool IsHashValid => Hash.ToHexString().StartsWith(new string('0', Difficulty));

    public bool IsSignatureValid => Voter.Verify(SignaturePayload, Signature);

    public Vote(Voter voter, int partyId, long timestamp) : this()
    {
        Voter = voter;
        PartyId = partyId;
        Timestamp = timestamp;

        Signature = voter.HasPrivateKey
            ? voter.Sign(SignaturePayload)
            : Signature;
    }

    public bool VerifySignature(byte[] sig) => Voter.Verify(SignaturePayload, sig);

    /// <summary>
    /// This will mine the vote on the client side, as proof of work.
    /// this is an attempt at a prevention of the sybil attack counter
    /// </summary>
    /// <returns>Mined Vote</returns>
    [Pure]
    public Vote Mine()
    {
        if (IsHashValid) return this;
        var foundNonce = Miner.Mine(HashPayload, Difficulty);
        return this with { Nonce = foundNonce };
    }

    public byte[] SignaturePayload
    {
        get
        {
            var buffer = new List<byte>();
            buffer.AddRange(BitConverter.GetBytes(PartyId));
            buffer.AddRange(BitConverter.GetBytes(Timestamp));
            return buffer.ToArray();
        }
    }

    private byte[] HashPayload
    {
        get
        {
            var buffer = new List<byte>();
            buffer.AddRange(Voter.Address.ToBytesFromHex());
            buffer.AddRange(Signature);
            buffer.AddRange(BitConverter.GetBytes(PartyId));
            buffer.AddRange(BitConverter.GetBytes(Timestamp));
            buffer.AddRange(BitConverter.GetBytes(Nonce));
            return buffer.ToArray();
        }
    }
}