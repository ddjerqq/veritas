using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Test.Infrastructure.Data.Models;

[Index(nameof(PublicKey), IsUnique = true)]
public class Voter
{
    [Key]
    [Column("addr")]
    [StringLength(44)]
    public string Address { get; init; } = default!;

    [Column("pkey")]
    [StringLength(182)]
    public string PublicKey { get; init; } = default!;

    public string Sign(byte[] data) => HMACSHA512.HashData(Address.ToBytesFromHex(), data).ToHexString();
}