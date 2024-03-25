using System.ComponentModel;
using Domain.ValueObjects;

namespace Test.Domain.Entities;

[EditorBrowsable(EditorBrowsableState.Never)]
internal class TestVoter
{
    private static readonly Voter Voter = Voter.NewVoter();

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
        Assert.That(Voter.Verify(payload, signature!), Is.True);
    }

    [Test]
    [Parallelizable]
    public void TestTwoVotersHaveDifferentSignatures()
    {
        var payload = "aaa"u8.ToArray();
        var signature = Voter.Sign(payload);

        var voter = Voter.NewVoter();

        Assert.That(Voter.Verify(payload, signature!), Is.True);
        Assert.That(voter.Verify(payload, signature!), Is.False);
    }

    [Test]
    [Parallelizable]
    public void TestLoadedPublicKeyCanVerifySignature()
    {
        var payload = "aaa"u8.ToArray();
        var signature = Voter.Sign(payload);

        var voter = Voter.FromPubKey(Voter.PublicKey);

        Assert.That(voter.Verify(payload, signature!), Is.True);
    }

    [Test]
    [Parallelizable]
    public void TestSigningWithoutSecretKeyThrows()
    {
        var voter = Voter.FromPubKey(Voter.PublicKey);
        Assert.Throws<InvalidOperationException>(() => voter.Sign("aaa"u8.ToArray()));
    }

    [Test]
    [Parallelizable]
    public void TestAddressGetHashCode()
    {
        Assert.That(Voter.GetHashCode(), Is.EqualTo(Voter.Address.GetHashCode()));
    }
}