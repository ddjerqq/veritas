using Domain.Common;
using Domain.Entities;

namespace Test.Domain.Entities;

internal class VoteTest
{
    [Test]
    [Parallelizable]
    public void TestVoteHash()
    {
        var vote = Vote.NewVote(Voter.NewVoter(), 0, DateTime.UtcNow);
        var hash = vote.Hash;

        Console.WriteLine(hash);
        Assert.That(hash, Is.Not.Null);
    }

    [Test]
    [Parallelizable]
    public void TestVoteSignature()
    {
        var vote = Vote.NewVote(Voter.NewVoter(), 0, DateTime.UtcNow);

        Console.WriteLine($"Vote Hash: {vote.Hash}");
        Assert.That(vote.Hash, Is.Not.Null);

        Console.WriteLine($"Vote Signature: {vote.Signature}");
        Assert.That(vote.Signature, Is.Not.Null);

        var isValid = vote.VerifySignature(vote.Signature.ToBytesFromHex());
        Assert.That(isValid, Is.True);
    }

    // [Test]
    // [NonParallelizable]
    // public void TestVoteMine()
    // {
    //     var vote = Vote.NewVote(Voter.NewVoter(), 0, DateTime.UtcNow);
    //
    //     var watch = Stopwatch.StartNew();
    //     vote.Mine();
    //     watch.Stop();
    //
    //     Console.WriteLine(vote.Nonce.ToString("N0"));
    //     Console.WriteLine(watch.Elapsed.ToString("c"));
    //     Console.WriteLine(vote);
    //     Console.WriteLine(vote.Hash);
    //
    //     Assert.That(vote.IsHashValid, Is.True);
    // }
}