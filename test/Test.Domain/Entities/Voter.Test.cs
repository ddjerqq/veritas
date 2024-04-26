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
}