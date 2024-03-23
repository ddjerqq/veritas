using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using Api.Common;

namespace Api.Data.Models;

public record Voter
{
    private ECDsa Algo { get; init; } = default!;
    private bool HasPrivateKey { get; init; }

    public string Address => SHA256.HashData(PublicKey).ToHexString()[22..64];

    public byte[] PublicKey => Algo.ExportSubjectPublicKeyInfo();

    public byte[] PrivateKey => Algo.ExportPkcs8PrivateKey();

    public static Voter NewVoter() => new()
    {
        Algo = ECDsa.Create(ECCurve.NamedCurves.nistP256),
        HasPrivateKey = true,
    };

    public static Voter FromPubKey(byte[] publicKey)
    {
        var voter = new Voter
        {
            Algo = ECDsa.Create(ECCurve.NamedCurves.nistP256),
            HasPrivateKey = false,
        };
        voter.Algo.ImportSubjectPublicKeyInfo(publicKey, out var pKeyBytesRead);

        if (publicKey.Length != pKeyBytesRead)
            throw new InvalidOperationException($"Invalid public key length, read {pKeyBytesRead} but the key is {publicKey.Length}");

        return voter;
    }

    [Pure]
    public bool TrySign(byte[] data, [NotNullWhen(true)] out byte[]? sig)
    {
        sig = default;

        if (HasPrivateKey)
            sig = Algo.SignData(data, HashAlgorithmName.SHA512);

        return HasPrivateKey;
    }

    public bool Verify(byte[] data, byte[] signature) => Algo.VerifyData(data, signature, HashAlgorithmName.SHA512);

    public override int GetHashCode() => Address.GetHashCode();

    public virtual bool Equals(Voter? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Address == other.Address;
    }
}