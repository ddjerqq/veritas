using System.Security.Cryptography;
using Domain.Common;
using Domain.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Test.Domain.Entities;

internal class TestVoter
{
    private static readonly Voter Voter = Voter.NewVoter();

    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        Converters =
        {
            new StringEnumConverter(),
        },
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy(),
        },
    };

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Environment.SetEnvironmentVariable("KEY_DERIVATION__SALT", "d7397fac-9506-4d7e-8ae5-bb64f659a49a");
    }

    [OneTimeTearDown]
    public void OneTimeTeardown()
    {
        Voter.Dispose();
    }

    [Test]
    [Parallelizable]
    public void TestAddressDoesNotChangeAfterSerde()
    {
        var json = JsonConvert.SerializeObject(Voter, JsonSerializerSettings);
        Console.WriteLine(json);

        var deserialized = JsonConvert.DeserializeObject<Voter>(json, JsonSerializerSettings);

        Assert.Multiple(() =>
        {
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(deserialized!.Address, Is.EqualTo(Voter.Address));
        });
    }

    [Test]
    [Parallelizable]
    public void TestSignatureChangesEvenOnSameData()
    {
        var payload = "aaa"u8.ToArray();
        var signatureA = Voter.Sign(payload);
        var signatureB = Voter.Sign(payload);

        Console.WriteLine(signatureA.ToHexString());
        Console.WriteLine(signatureB.ToHexString());

        Assert.That(signatureA.ToHexString(), Is.Not.EqualTo(signatureB.ToHexString()));
    }

    [Test]
    [Parallelizable]
    public void TestSign()
    {
        var payload = "aaa"u8.ToArray();
        var signature = Voter.Sign(payload);
        Console.WriteLine(signature);
        Assert.That(Voter.Verify(payload, signature), Is.True);
    }

    [Test]
    [Parallelizable]
    public void TestLoadedPublicKeyCanVerifySignature()
    {
        var payload = "aaa"u8.ToArray();
        var signature = Voter.Sign(payload);
        var voter = Voter.FromPublicKey(Voter.PublicKey);
        Assert.That(voter.Verify(payload, signature), Is.True);
    }

    [Test]
    [Parallelizable]
    public void TestSigningWithoutSecretKeyThrows()
    {
        var voter = Voter.FromPublicKey(Voter.PublicKey);
        Assert.Throws<InvalidOperationException>(() => voter.Sign("aaa"u8.ToArray()));
    }

    [Test]
    [Parallelizable]
    public void TestCanStillSignAfterJsonSerde()
    {
        var payload = "aaa"u8.ToArray();
        var signature = Voter.Sign(payload);

        var json = JsonConvert.SerializeObject(Voter, JsonSerializerSettings);
        Console.WriteLine(json);

        var deserialized = JsonConvert.DeserializeObject<Voter>(json, JsonSerializerSettings);

        Assert.Multiple(() =>
        {
            Assert.That(deserialized, Is.Not.Null);

            Assert.That(deserialized!.Address, Is.EqualTo(Voter.Address));
            Assert.That(deserialized.PublicKey, Is.EqualTo(Voter.PublicKey));
            Assert.That(deserialized.PrivateKey, Is.Null);

            Assert.That(deserialized.Verify(payload, signature), Is.True);
        });
    }

    [Test]
    [Parallelizable]
    public void TestCanExportAndImport()
    {
        var voter = Voter.NewVoter();
        var importedVoter = Voter.FromKeyPair(voter.PublicKey, voter.PrivateKey!);

        Assert.Multiple(() =>
        {
            Assert.That(importedVoter.Address, Is.EqualTo(voter.Address));
            Assert.That(importedVoter.PublicKey, Is.EqualTo(voter.PublicKey));
            Assert.That(importedVoter.PrivateKey, Is.EqualTo(voter.PrivateKey));
        });

        // signing the same data will not be the same, however, verifying the said data must be possible
        var payload = "aaa"u8.ToArray();
        var signature = voter.Sign(payload);
        Assert.That(importedVoter.Verify(payload, signature), Is.True);

        // test the inverse
        var importedSignature = importedVoter.Sign(payload);
        Assert.That(voter.Verify(payload, importedSignature), Is.True);
    }

    [Test]
    [Parallelizable]
    public void TestKeyDerivation()
    {
        const string passphrase = "moon ball archer hell salt room floor leaf can file plan hex";
        const string expectedSKey = "005d8938f02b57b180334909e7e6fe68088ee54ab5435d1170dc7f3dc446ce32";
        const string expectedPKey =
            "ac293b8e19913cf0ad43af21455146cc2d708699f050646ec78e184a1a94c40a825eb23e0c531b2a8008821fd6cda91caf597138d7530e2395994b6f03d35b1e";
        const int iterations = 10000;
        const int derivedKeyLength = 32;
        const string salt = "salt";

        var saltBytes = salt.Select(c => (byte)c).ToArray();

        using var pbkdf2 = new Rfc2898DeriveBytes(passphrase, saltBytes, iterations, HashAlgorithmName.SHA256);
        var derivedKey = pbkdf2.GetBytes(derivedKeyLength);

        using var ecdsa = ECDsa.Create();

        ecdsa.ImportParameters(new ECParameters
        {
            Curve = ECCurve.NamedCurves.nistP256,
            D = derivedKey,
        });

        var sKey = ecdsa.ExportParameters(true).D!.ToHexString();
        var pKey = $"{ecdsa.ExportParameters(false).Q.X!.ToHexString()}{ecdsa.ExportParameters(true).Q.Y!.ToHexString()}";

        Console.WriteLine($"S key: {sKey}");
        Console.WriteLine($"P key: {pKey}");

        Assert.Multiple(() =>
        {
            Assert.That(sKey, Is.EqualTo(expectedSKey));
            Assert.That(pKey, Is.EqualTo(expectedPKey));
        });
    }

    [Test]
    [Parallelizable]
    public void TestFromSeed()
    {
        var voter = Voter.FromSeed("pangea seven human ball ray golem generate");
        Console.WriteLine(voter.PrivateKey);
        Console.WriteLine(voter.PublicKey);
    }
}