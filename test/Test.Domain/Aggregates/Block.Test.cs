using System.Diagnostics;
using Domain.Aggregates;
using Domain.Common;
using Domain.ValueObjects;

namespace Test.Domain.Aggregates;

internal class BlockTest
{
    [Test]
    [NonParallelizable]
    public void TestBlockMine()
    {
        // 1 difficulty means 2 leading zeroes
        var block = Block.Genesis().Next();

        for (int i = 0; i < Block.VoteLimit; i++)
            block.TryAddVote(new Vote(Voter.NewVoter(), 0, 0));

        var stopwatch = Stopwatch.StartNew();
        var minedBlock = block.Mine();
        stopwatch.Stop();

        Console.WriteLine(minedBlock.Nonce.ToString("N0"));
        Console.WriteLine(minedBlock.Hash.ToHexString());
        Console.WriteLine(stopwatch.Elapsed.ToString("c"));
        Assert.That(minedBlock.IsHashValid, Is.True);
    }

    [Test]
    [Parallelizable]
    public void TestBlockHash()
    {
        var block = Block.Genesis();
        var hash = block.Hash.ToHexString();
        Console.WriteLine(hash);
    }

    [Test]
    [Parallelizable]
    public void TestCalculateMerkleRootForVotes()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var block = Block.Genesis();

        block.TryAddVote(new Vote(Voter.NewVoter(), 0, timestamp));
        block.TryAddVote(new Vote(Voter.NewVoter(), 0, timestamp));
        block.TryAddVote(new Vote(Voter.NewVoter(), 0, timestamp));
        block.TryAddVote(new Vote(Voter.NewVoter(), 0, timestamp));

        Console.WriteLine(block.MerkleRoot.ToHexString());
    }

    [Test]
    [Parallelizable]
    public void TestHashBlockWithVotes()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var block = Block.Genesis();

        block.TryAddVote(new Vote(Voter.NewVoter(), 0, timestamp));
        block.TryAddVote(new Vote(Voter.NewVoter(), 0, timestamp));
        block.TryAddVote(new Vote(Voter.NewVoter(), 0, timestamp));
        block.TryAddVote(new Vote(Voter.NewVoter(), 0, timestamp));

        Console.WriteLine(block.MerkleRoot.ToHexString());

        // mine operation
        var minedBlock = block.Mine();

        Console.WriteLine(minedBlock.Nonce.ToString("N0"));
        Console.WriteLine(minedBlock.Hash.ToHexString());

        Assert.That(minedBlock.IsHashValid, Is.True);
    }

    [Test]
    [Parallelizable]
    public void TestHashBlockWithMaxVotes()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var block = Block.Genesis();

        block.TryAddVote(new Vote(Voter.NewVoter(), 0, timestamp));
        block.TryAddVote(new Vote(Voter.NewVoter(), 0, timestamp));
        block.TryAddVote(new Vote(Voter.NewVoter(), 0, timestamp));
        block.TryAddVote(new Vote(Voter.NewVoter(), 0, timestamp));

        Console.WriteLine(block.MerkleRoot.ToHexString());

        // mine operation
        var minedBlock = block.Mine();

        Console.WriteLine(minedBlock.Nonce.ToString("N0"));
        Console.WriteLine(minedBlock.Hash.ToHexString());

        Assert.That(minedBlock.IsHashValid, Is.True);
    }

    [Test]
    [Parallelizable]
    public void TestCantAddMoreThanLimit()
    {
        var block = Block.Genesis();

        for (var i = 0; i < Block.VoteLimit; i++)
            block.TryAddVote(new Vote(Voter.NewVoter(), 0, 0));

        Assert.That(block.TryAddVote(new Vote(Voter.NewVoter(), 0, 0)), Is.False);
    }
}