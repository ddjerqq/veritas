using System.Diagnostics;
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

        Console.WriteLine($"Vote Signature: {vote.Signature.ToHexString()}");
        Assert.That(vote.Signature.ToHexString(), Is.Not.Null);

        var isValid = vote.VerifySignature(vote.Signature);
        Assert.That(isValid, Is.True);
    }

    [Test]
    [NonParallelizable]
    public void TestVoteMine()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var vote = new Vote(Voter.NewVoter(), 0, timestamp);

        var watch = Stopwatch.StartNew();
        var minedVote = vote.Mine();
        watch.Stop();

        Console.WriteLine(minedVote.Nonce.ToString("N0"));
        Console.WriteLine(watch.Elapsed.ToString("c"));
        Console.WriteLine(minedVote);
        Console.WriteLine(minedVote.Hash.ToHexString());

        Assert.That(minedVote.IsHashValid, Is.True);
    }
}