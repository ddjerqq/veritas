using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Test.Infrastructure.Data.Models;

[Index(nameof(PartyId))]
public class Vote
{
    [Key]
    [StringLength(64)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Hash
    {
        get => SHA256.HashData(this.GetHashPayload()).ToHexString();
        protected init => _ = value;
    }

    public long Nonce { get; init; }

    public int PartyId { get; init; }

    [StringLength(44)]
    public string VoterAddress { get; set; } = default!;

    public Voter Voter { get; init; } = default!;

    [StringLength(128)]
    public string Signature
    {
        get => Voter.Sign(this.GetSignaturePayload());
        protected init => _ = value;
    }

    public long Timestamp { get; init; }

    [NotMapped]
    public DateTime UtcDateTime => DateTimeOffset.FromUnixTimeMilliseconds(Timestamp).UtcDateTime;

    public long? BlockIndex { get; set; }

    public Block? Block { get; set; }
}

public static class VoteExt
{
    public static byte[] GetSignaturePayload(this Vote vote)
    {
        var buffer = new List<byte>();
        buffer.AddRange(BitConverter.GetBytes(vote.PartyId));
        buffer.AddRange(BitConverter.GetBytes(vote.Timestamp));
        return buffer.ToArray();
    }

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