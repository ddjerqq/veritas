using System.Diagnostics;
using Domain.Common;
using Domain.Entities;

namespace Test.Domain.Entities;

internal class VoteTest
{
    [Test]
    [Parallelizable]
    public void TestVoteHash()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var vote = Vote.NewVote(Voter.NewVoter(), 0, timestamp);
        var hash = vote.Hash.ToHexString();

        Console.WriteLine(hash);
        Assert.That(hash, Is.Not.Null);
    }

    [Test]
    [Parallelizable]
    public void TestVoteSignature()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var vote = Vote.NewVote(Voter.NewVoter(), 0, timestamp);

        Console.WriteLine($"Vote Hash: {vote.Hash.ToHexString()}");
        Assert.That(vote.Hash.ToHexString(), Is.Not.Null);

        Console.WriteLine($"Vote Signature: {vote.Signature}");
        Assert.That(vote.Signature, Is.Not.Null);

        var isValid = vote.VerifySignature(vote.Signature.ToBytesFromHex());
        Assert.That(isValid, Is.True);
    }

    [Test]
    [NonParallelizable]
    public void TestVoteMine()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var vote = Vote.NewVote(Voter.NewVoter(), 0, timestamp);

        var watch = Stopwatch.StartNew();
        vote.Mine();
        watch.Stop();

        Console.WriteLine(vote.Nonce.ToString("N0"));
        Console.WriteLine(watch.Elapsed.ToString("c"));
        Console.WriteLine(vote);
        Console.WriteLine(vote.Hash.ToHexString());

        Assert.That(vote.IsHashValid, Is.True);
    }
}