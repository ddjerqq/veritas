using System.Security.Cryptography;
using System.Text;
using Domain.Common;
using SJsonIgnore = System.Text.Json.Serialization.JsonIgnoreAttribute;
using NJsonIgnore = Newtonsoft.Json.JsonIgnoreAttribute;

namespace Domain.Entities;

public sealed class Voter : IDisposable
{
    private const int KeyDerivationIterations = 10000;

    private static byte[] KeyDerivationSalt =>
        Encoding.UTF8.GetBytes(
            Environment.GetEnvironmentVariable("KEY_DERIVATION__SALT")
            ?? throw new InvalidOperationException("KEY_DERIVATION__SALT is not set"));

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

    public ICollection<Vote> Votes { get; init; } = [];

    public static Voter NewVoter()
    {
        var dsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);

        var parameters = dsa.ExportParameters(includePrivateParameters: true);

        var publicKey = new byte[64];
        parameters.Q.X!.CopyTo(publicKey, 0);
        parameters.Q.Y!.CopyTo(publicKey, 32);

        return new Voter
        {
            Dsa = dsa,
            PublicKey = publicKey.ToHexString(),
            PrivateKey = parameters.D!.ToHexString(),
        };
    }

    public static Voter FromSeed(string seed)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(seed, KeyDerivationSalt, KeyDerivationIterations, HashAlgorithmName.SHA256);
        var derivedKey = pbkdf2.GetBytes(32);

        using var dsa = ECDsa.Create();

        dsa.ImportParameters(new ECParameters
        {
            Curve = ECCurve.NamedCurves.nistP256,
            D = derivedKey,
        });

        var parameters = dsa.ExportParameters(true);

        var publicKey = new byte[64];
        parameters.Q.X!.CopyTo(publicKey, 0);
        parameters.Q.Y!.CopyTo(publicKey, 32);

        return new Voter
        {
            Dsa = ECDsa.Create(parameters),
            PublicKey = publicKey.ToHexString(),
            PrivateKey = parameters.D!.ToHexString(),
        };
    }

    public static Voter FromPublicKey(string publicKey) => FromKeyPair(publicKey, null);

    public static Voter FromKeyPair(string publicKey, string? privateKey)
    {
        var parameters = CreateParameters(publicKey, privateKey);
        var dsa = ECDsa.Create(parameters);

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
        // for when we json de-serialize, the DSA will have newly generated key info
        var parameters = CreateParameters(PublicKey, PrivateKey);
        Dsa.ImportParameters(parameters);

        return Dsa.VerifyData(data, signature, HashAlgorithmName.SHA512);
    }

    /// <summary>
    /// Generates the signature of the address which is used when verifying the voter's identity
    /// </summary>
    public string SignAddress() => Sign(Address.ToBytesFromHex()).ToHexString();

    public bool VerifyAddressSignature(string signature) => Verify(Address.ToBytesFromHex(), signature.ToBytesFromHex());

    private static ECParameters CreateParameters(string publicKey, string? privateKey) => new()
    {
        Curve = ECCurve.NamedCurves.nistP256,
        D = privateKey?.ToBytesFromHex(),
        Q = new ECPoint
        {
            X = publicKey.ToBytesFromHex().Take(32).ToArray(),
            Y = publicKey.ToBytesFromHex().Skip(32).ToArray(),
        },
    };

    public void Dispose() => Dsa.Dispose();
}