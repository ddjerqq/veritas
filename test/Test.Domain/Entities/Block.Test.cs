using System.Diagnostics;
using Domain.Common;
using Domain.Entities;

namespace Test.Domain.Entities;

internal class BlockTest
{
    // [Test]
    // [NonParallelizable]
    // public void TestBlockMine()
    // {
    //     // 1 difficulty means 2 leading zeroes
    //     var block = Block.GenesisBlock().NextBlock();
    //
    //     for (var i = 0; i < 2; i++)
    //     {
    //         var vote = Vote.NewVote(Voter.NewVoter(), 0, 0L.ToUtcDateTime());
    //         vote.Mine();
    //
    //         Assert.Multiple(() =>
    //         {
    //             Assert.That(vote.IsHashValid, Is.True);
    //             Assert.That(vote.IsSignatureValid, Is.True);
    //         });
    //
    //         block.AddVote(vote);
    //     }
    //
    //     var stopwatch = Stopwatch.StartNew();
    //     block.Mine();
    //     stopwatch.Stop();
    //
    //     Console.WriteLine(block.Nonce.ToString("N0"));
    //     Console.WriteLine(block.Hash);
    //     Console.WriteLine(stopwatch.Elapsed.ToString("c"));
    //
    //     Assert.That(block.IsHashValid, Is.True);
    // }

    [Test]
    [Parallelizable]
    public void TestBlockHash()
    {
        var block = Block.GenesisBlock();
        Console.WriteLine(block.Hash);
    }
}