using System.Security.Cryptography;
using Domain.Common;
using Serilog;
using SJsonIgnore = System.Text.Json.Serialization.JsonIgnoreAttribute;
using NJsonIgnore = Newtonsoft.Json.JsonIgnoreAttribute;

namespace Domain.Entities;

public sealed class Voter : IDisposable
{
    private ECDsa Dsa { get; init; } = ECDsa.Create(ECCurve.NamedCurves.nistP256);

    public string Address
    {
        get => "0x" + SHA256.HashData(PublicKey.ToBytesFromHex()).ToHexString()[22..64];
        // ReSharper disable once UnusedMember.Local for EF Core
        private init => _ = value;
    }

    public string PublicKey { get; init; } = default!;

    [NJsonIgnore]
    [SJsonIgnore]
    public string? PrivateKey { get; init; }

    public void Dispose()
    {
        Dsa.Dispose();
    }

    public static Voter NewVoter()
    {
        var dsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);

        return new Voter
        {
            Dsa = dsa,
            PublicKey = dsa.ExportSubjectPublicKeyInfo().ToHexString(),
            PrivateKey = dsa.ExportPkcs8PrivateKey().ToHexString(),
        };
    }

    public static Voter FromPublicKey(string publicKey)
    {
        var dsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);

        try
        {
            dsa.ImportSubjectPublicKeyInfo(publicKey.ToBytesFromHex(), out _);
        }
        catch (CryptographicException ex)
        {
            Log.Error(ex, "Invalid public key");
            throw new InvalidOperationException("Invalid public key", ex);
        }

        var voter = new Voter
        {
            Dsa = dsa,
            PublicKey = publicKey,
        };

        return voter;
    }

    public static Voter FromKeyPair(string publicKey, string privateKey)
    {
        var dsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);

        var parameters = dsa.ExportParameters(true);

        // TODO either fix this parameter export bullshit
        ECParameters paramers = new ECParameters()
        {
            Curve = ECCurve.NamedCurves.nistP256,
        };
        // ECDsa.Create()

        try
        {
            dsa.ImportSubjectPublicKeyInfo(publicKey.ToBytesFromHex(), out _);
            dsa.ImportPkcs8PrivateKey(privateKey.ToBytesFromHex(), out _);
        }
        catch (CryptographicException ex)
        {
            Log.Error(ex, "Invalid public key");
            throw new InvalidOperationException("Invalid public key", ex);
        }

        var voter = new Voter
        {
            Dsa = dsa,
            PublicKey = publicKey,
            PrivateKey = privateKey,
        };

        return voter;
    }

    public byte[] Sign(byte[] data)
    {
        if (string.IsNullOrWhiteSpace(PrivateKey)) throw new InvalidOperationException("Cannot sign without private key");
        return Dsa.SignData(data, HashAlgorithmName.SHA512);
    }

    public bool Verify(byte[] data, byte[] signature)
    {
        Dsa.ImportSubjectPublicKeyInfo(PublicKey.ToBytesFromHex(), out _);
        return Dsa.VerifyData(data, signature, HashAlgorithmName.SHA512);
    }

    /// <summary>
    /// Generates the signature of the address which is used when verifying the voter's identity
    /// </summary>
    public string GenerateAddressSignature() => Sign(Address.ToBytesFromHex()).ToHexString();
}