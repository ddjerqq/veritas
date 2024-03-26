using System.Security.Cryptography;
using Domain.Common;

namespace Domain.ValueObjects;

public record Vote()
{
    public Voter Voter { get; init; } = default!;

    public int PartyId { get; init; }

    public long Timestamp { get; init; }

    public byte[] Signature { get; init; } = default!;

    public byte[] Hash => SHA256.HashData(HashPayload);

    public Vote(Voter voter, int partyId, long timestamp) : this()
    {
        Voter = voter;
        PartyId = partyId;
        Timestamp = timestamp;

        Signature = voter.HasPrivateKey
            ? voter.Sign(GetSignaturePayload(PartyId, Timestamp))
            : Signature;
    }

    public bool VerifySignature(byte[] sig) => Voter.Verify(GetSignaturePayload(PartyId, Timestamp), sig);

    public static byte[] GetSignaturePayload(int partyId, long timestamp)
    {
        var buffer = new List<byte>();
        buffer.AddRange(BitConverter.GetBytes(partyId));
        buffer.AddRange(BitConverter.GetBytes(timestamp));
        return buffer.ToArray();
    }

    private byte[] HashPayload
    {
        get
        {
            var buffer = new List<byte>();
            buffer.AddRange(Voter.Address.ToBytesFromHex());
            buffer.AddRange(BitConverter.GetBytes(PartyId));
            buffer.AddRange(BitConverter.GetBytes(Timestamp));
            buffer.AddRange(Signature);
            return buffer.ToArray();
        }
    }
}