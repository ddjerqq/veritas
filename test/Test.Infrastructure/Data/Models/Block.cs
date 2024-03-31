using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Mrkl = Domain.Common.MerkleRoot;

namespace Test.Infrastructure.Data.Models;

[Index(nameof(Hash), IsUnique = true)]
public class Block
{
    private readonly List<Vote> _votes = [];

    [Key]
    [Column("idx", Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Index { get; init; }

    [StringLength(64)]
    public string Hash
    {
        get => SHA256.HashData(this.GetHashPayload()).ToHexString();
        protected init => _ = value;
    }

    public long Nonce { get; init; }

    [StringLength(64)]
    public string MerkleRoot
    {
        get => Mrkl.BuildMerkleRoot(_votes.Select(v => v.Hash.ToBytesFromHex())).ToHexString();
        protected init => _ = value;
    }

    [StringLength(64)]
    public string PreviousHash { get; init; } = default!;

    public ICollection<Vote> Votes => _votes.AsReadOnly();

    public void AddVote(Vote vote) => _votes.Add(vote);
}

public static class BlockExt
{
    public static byte[] GetHashPayload(this Block block)
    {
        var buffer = new List<byte>();
        buffer.AddRange(BitConverter.GetBytes(block.Index));
        buffer.AddRange(block.PreviousHash.ToBytesFromHex());
        buffer.AddRange(block.MerkleRoot.ToBytesFromHex());
        buffer.AddRange(BitConverter.GetBytes(block.Nonce));
        return buffer.ToArray();
    }
}