using System.Security.Cryptography;
using Domain.Common;
using SJsonIgnore = System.Text.Json.Serialization.JsonIgnoreAttribute;
using NJsonIgnore = Newtonsoft.Json.JsonIgnoreAttribute;

namespace Domain.Entities;

public sealed class Vote
{
    public const int Difficulty = 4;

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local for EF Core
    public required string Hash { get; set; }

    [NJsonIgnore]
    [SJsonIgnore]
    public string VoterAddress { get; init; } = default!;

    public Voter Voter { get; init; } = default!;

    public int PartyId { get; init; }

    public DateTime Timestamp { get; init; }

    [NJsonIgnore]
    [SJsonIgnore]
    public long UnixTimestampMs => Timestamp.ToUnixMs();

    public string Signature { get; init; } = default!;

    public long Nonce { get; set; }

    [NJsonIgnore]
    [SJsonIgnore]
    public long? BlockIndex { get; set; }

    [NJsonIgnore]
    [SJsonIgnore]
    public Block? Block { get; set; }

    [NJsonIgnore]
    [SJsonIgnore]
    public bool IsHashValid => Hash.StartsWith(new string('0', Difficulty));

    [NJsonIgnore]
    [SJsonIgnore]
    public bool IsSignatureValid => Voter.Verify(this.GetSignaturePayload(), Signature.ToBytesFromHex());

    public static Vote NewVote(Voter voter, int partyId, DateTime timestamp)
    {
        var signature = voter.Sign(VoteExt.GetSignaturePayload(partyId, timestamp.ToUnixMs())).ToHexString();
        var hashPayload = VoteExt.GetHashPayload(voter.Address, partyId, timestamp.ToUnixMs(), 0);
        var hash = SHA256.HashData(hashPayload).ToHexString();

        return new Vote
        {
            Hash = hash,
            Voter = voter,
            PartyId = partyId,
            Timestamp = timestamp,
            Signature = signature,
        };
    }

    public bool VerifySignature(byte[] sig)
    {
        return Voter.Verify(this.GetSignaturePayload(), sig);
    }

    public void Mine()
    {
        if (IsHashValid) return;
        var foundNonce = Miner.Mine(this.GetHashPayload(), Difficulty);

        Nonce = foundNonce;
        Hash = SHA256.HashData(this.GetHashPayload()).ToHexString();
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

    public static byte[] GetSignaturePayload(this Vote vote)
    {
        return GetSignaturePayload(vote.PartyId, vote.UnixTimestampMs);
    }

    // we will not be including the signature in the hash payload because it is not possible
    // to sign from the client side, as WASM does not support crypto libraries.
    // in a perfect world we would tho
    public static byte[] GetHashPayload(string address, int partyId, long timestamp, long nonce)
    {
        var buffer = new List<byte>();
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract for EF Core
        buffer.AddRange(address.ToBytesFromHex());
        buffer.AddRange(BitConverter.GetBytes(partyId));
        buffer.AddRange(BitConverter.GetBytes(timestamp));
        buffer.AddRange(BitConverter.GetBytes(nonce));
        return buffer.ToArray();
    }

    public static byte[] GetHashPayload(this Vote vote) => GetHashPayload(vote.Voter?.Address ?? "", vote.PartyId, vote.UnixTimestampMs, vote.Nonce);
}