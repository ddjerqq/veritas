using System.Diagnostics;
using Domain.Entities;

namespace Test.Domain.Entities;

internal class BlockTest
{
    [Test]
    [NonParallelizable]
    public void TestBlockMine()
    {
        // 1 difficulty means 2 leading zeroes
        var block = Block.GenesisBlock().NextBlock();

        for (int i = 0; i < 256; i++)
            block.TryAddVote(Vote.NewVote(Voter.NewVoter(), 0, 0));

        var stopwatch = Stopwatch.StartNew();
        block.Mine();
        stopwatch.Stop();

        Console.WriteLine(block.Nonce.ToString("N0"));
        Console.WriteLine(block.Hash);
        Console.WriteLine(stopwatch.Elapsed.ToString("c"));
        Assert.That(block.IsHashValid, Is.True);
    }

    [Test]
    [Parallelizable]
    public void TestBlockHash()
    {
        var block = Block.GenesisBlock();
        Console.WriteLine(block.Hash);
    }

    [Test]
    [Parallelizable]
    public void TestCalculateMerkleRootForVotes()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var block = Block.GenesisBlock();

        block.TryAddVote(Vote.NewVote(Voter.NewVoter(), 0, timestamp));
        block.TryAddVote(Vote.NewVote(Voter.NewVoter(), 0, timestamp));
        block.TryAddVote(Vote.NewVote(Voter.NewVoter(), 0, timestamp));
        block.TryAddVote(Vote.NewVote(Voter.NewVoter(), 0, timestamp));

        Console.WriteLine(block.MerkleRoot);
    }

    [Test]
    [Parallelizable]
    public void TestHashBlockWithVotes()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var block = Block.GenesisBlock();

        block.TryAddVote(Vote.NewVote(Voter.NewVoter(), 0, timestamp));
        block.TryAddVote(Vote.NewVote(Voter.NewVoter(), 0, timestamp));
        block.TryAddVote(Vote.NewVote(Voter.NewVoter(), 0, timestamp));
        block.TryAddVote(Vote.NewVote(Voter.NewVoter(), 0, timestamp));

        block.Mine();

        Console.WriteLine(block.MerkleRoot);

        Console.WriteLine(block.Nonce.ToString("N0"));
        Console.WriteLine(block.Hash);

        Assert.That(block.IsHashValid, Is.True);
    }

    [Test]
    [Parallelizable]
    public void TestHashBlockWithMaxVotes()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var block = Block.GenesisBlock();

        block.TryAddVote(Vote.NewVote(Voter.NewVoter(), 0, timestamp));
        block.TryAddVote(Vote.NewVote(Voter.NewVoter(), 0, timestamp));
        block.TryAddVote(Vote.NewVote(Voter.NewVoter(), 0, timestamp));
        block.TryAddVote(Vote.NewVote(Voter.NewVoter(), 0, timestamp));

        Console.WriteLine(block.MerkleRoot);

        // mine operation
        block.Mine();

        Console.WriteLine(block.Nonce.ToString("N0"));
        Console.WriteLine(block.Hash);

        Assert.That(block.IsHashValid, Is.True);
    }
}