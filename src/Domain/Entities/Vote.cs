using System.Security.Cryptography;
using Domain.Common;
using Newtonsoft.Json;

namespace Domain.Entities;

public class Vote
{
    private const int Difficulty = 6;

    public byte[] Hash
    {
        get => SHA256.HashData(this.GetHashPayload());
        // ReSharper disable once UnusedMember.Local for EF Core
        private init => _ = value;
    }

    public string VoterAddress { get; init; } = default!;

    public Voter Voter { get; init; } = default!;

    public int PartyId { get; init; }

    public long Timestamp { get; init; }

    public string Signature { get; init; } = default!;

    public long Nonce { get; set; }

    public long? BlockIndex { get; set; }

    public Block? Block { get; set; }

    [JsonIgnore]
    public bool IsHashValid => Hash.ToHexString().StartsWith(new string('0', Difficulty));

    [JsonIgnore]
    public bool IsSignatureValid => Voter.Verify(this.GetSignaturePayload(), Signature.ToBytesFromHex());

    public static Vote NewVote(Voter voter, int partyId, long timestamp)
    {
        return new Vote
        {
            Voter = voter,
            PartyId = partyId,
            Timestamp = timestamp,
            Signature = voter.Sign(VoteExt.GetSignaturePayload(partyId, timestamp)).ToHexString(),
        };
    }

    public bool VerifySignature(byte[] sig) => Voter.Verify(this.GetSignaturePayload(), sig);

    public void Mine()
    {
        if (IsHashValid) return;
        var foundNonce = Miner.Mine(this.GetHashPayload(), Difficulty);
        Nonce = foundNonce;
    }
}

public static class VoteExt
{
    public static byte[] GetSignaturePayload(int partyId, long timestamp)
    {
        var buffer = new List<byte>();
        buffer.AddRange(BitConverter.GetBytes(partyId));
        buffer.AddRange(BitConverter.GetBytes(timestamp));
        return buffer.ToArray();
    }

    public static byte[] GetSignaturePayload(this Vote vote) => GetSignaturePayload(vote.PartyId, vote.Timestamp);

    public static byte[] GetHashPayload(this Vote vote)
    {
        var buffer = new List<byte>();
        buffer.AddRange(vote.Voter.Address.ToBytesFromHex());
        buffer.AddRange(vote.Signature.ToBytesFromHex());
        buffer.AddRange(BitConverter.GetBytes(vote.PartyId));
        buffer.AddRange(BitConverter.GetBytes(vote.Timestamp));
        buffer.AddRange(BitConverter.GetBytes(vote.Nonce));
        return buffer.ToArray();
    }
}