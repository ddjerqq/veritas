using Domain.Common;
using Domain.ValueObjects;

namespace Test.Domain.Entities;

internal class VoteTest
{
    [Test]
    [Parallelizable]
    public void TestVoteHash()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var vote = new Vote(Voter.NewVoter(), 0, timestamp);
        var hash = vote.Hash.ToHexString();

        Console.WriteLine(hash);
        Assert.That(hash, Is.Not.Null);
    }

    [Test]
    [Parallelizable]
    public void TestVoteSignature()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var vote = new Vote(Voter.NewVoter(), 0, timestamp);

        Console.WriteLine($"Vote Hash: {vote.Hash.ToHexString()}");
        Assert.That(vote.Hash.ToHexString(), Is.Not.Null);

        Console.WriteLine($"Vote Signature: {vote.Signature.ToBase64String()}");
        Assert.That(vote.Signature.ToBase64String(), Is.Not.Null);

        var isValid = vote.VerifySignature(vote.Signature);
        Assert.That(isValid, Is.True);
    }

    // [Test]
    // [Parallelizable]
    // public void TestThrowsOnInvalidHashAndSignature()
    // {
    //     var vote = new Vote(Voter.NewVoter(), 0, 0)!;
    //     var dto = (VoteDto)vote;
    //
    //     var badHash = dto with { Hash = new string('0', 64) };
    //     Assert.Throws<InvalidOperationException>(() => _ = (Vote)badHash);
    //
    //     var badSig = dto with { Signature = new string('0', 64) };
    //     Assert.Throws<InvalidOperationException>(() => _ = (Vote)badSig);
    // }
}