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
    public void TestSign()
    {
        var payload = "aaa"u8.ToArray();
        var signature = Voter.Sign(payload);
        Assert.That(Voter.Verify(payload, signature), Is.True);
    }

    [Test]
    [Parallelizable]
    public void TestTwoVotersHaveDifferentSignatures()
    {
        var payload = "aaa"u8.ToArray();
        var signature = Voter.Sign(payload);

        var voter = Voter.NewVoter();

        Assert.That(Voter.Verify(payload, signature), Is.True);
        Assert.That(voter.Verify(payload, signature), Is.False);
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
}