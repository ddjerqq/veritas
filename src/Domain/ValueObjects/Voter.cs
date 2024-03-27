﻿using System.Security.Cryptography;
using Domain.Common;

namespace Domain.ValueObjects;

public record Voter : IDisposable
{
    private ECDsa Dsa { get; init; } = default!;

    public bool HasPrivateKey { get; private init; }

    public string Address => "0x" + SHA256.HashData(PublicKey).ToHexString()[22..64];

    public byte[] PublicKey { get; private init; } = default!;

    // ReSharper disable once UnusedMember.Global, this will come in handy when saving the privatekey to the users local storage.
    public byte[]? PrivateKey { get; private init; }

    public static Voter NewVoter()
    {
        var dsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);

        return new Voter
        {
            Dsa = dsa,
            PublicKey = dsa.ExportSubjectPublicKeyInfo(),
            PrivateKey = dsa.ExportPkcs8PrivateKey(),
            HasPrivateKey = true,
        };
    }

    public static Voter FromPubKey(byte[] publicKey)
    {
        var dsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);

        try
        {
            dsa.ImportSubjectPublicKeyInfo(publicKey, out var pKeyBytesRead);

            if (publicKey.Length != pKeyBytesRead)
                throw new InvalidOperationException($"Invalid public key length, read {pKeyBytesRead} but the key is {publicKey.Length}");
        }
        catch (CryptographicException ex)
        {
            // TODO after we implement Serilog, log the exception info here,
            //  we should stay alert for users trying suspicious public keys
            throw new InvalidOperationException("Invalid public key", ex);
        }

        var voter = new Voter
        {
            Dsa = dsa,
            PublicKey = dsa.ExportSubjectPublicKeyInfo(),
            HasPrivateKey = false,
        };

        return voter;
    }

    public byte[] Sign(byte[] data)
    {
        if (!HasPrivateKey) throw new InvalidOperationException("Cannot sign without private key");
        return Dsa.SignData(data, HashAlgorithmName.SHA512);
    }

    public bool Verify(byte[] data, byte[] signature) => Dsa.VerifyData(data, signature, HashAlgorithmName.SHA512);

    public override int GetHashCode() => Address.GetHashCode();

    public virtual bool Equals(Voter? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Address == other.Address;
    }

    public void Dispose() => Dsa.Dispose();
}