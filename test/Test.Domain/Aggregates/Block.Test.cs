// using System.ComponentModel;
// using System.Diagnostics;
// using Domain.Aggregates;
// using Domain.Entities;
//
// namespace Test.Domain.Aggregates;
//
// [EditorBrowsable(EditorBrowsableState.Never)]
// internal class BlockTest
// {
//     [Test]
//     [Parallelizable]
//     public void TestBlockMine()
//     {
//         // 1 difficulty means 2 leading zeroes
//         var block = Block.Genesis().Next();
//         for (int i = 0; i < 256; i++)
//             block.AddVote(Vote.TryCreate(Voter.NewVoter(), 0, 0)!);
//
//         var stopwatch = Stopwatch.StartNew();
//         block = block.Mine();
//         stopwatch.Stop();
//
//         Console.WriteLine(block.Nonce);
//         Console.WriteLine(block.Hash.ToHexString());
//
//         Console.WriteLine(stopwatch.ElapsedMilliseconds);
//
//         Assert.That(block.IsHashValid, Is.True);
//     }
//
//     [Test]
//     [Parallelizable]
//     public void TestBlockHash()
//     {
//         var block = Block.Genesis();
//         var hash = block.Hash.ToHexString();
//         Console.WriteLine(hash);
//     }
//
//     [Test]
//     [Parallelizable]
//     public void TestCalculateMerkleRootForVotes()
//     {
//         var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
//
//         var block = Block.Genesis();
//
//         block.AddVote(Vote.TryCreate(Voter.NewVoter(), 0, timestamp)!);
//         block.AddVote(Vote.TryCreate(Voter.NewVoter(), 0, timestamp)!);
//         block.AddVote(Vote.TryCreate(Voter.NewVoter(), 0, timestamp)!);
//         block.AddVote(Vote.TryCreate(Voter.NewVoter(), 0, timestamp)!);
//
//         Console.WriteLine(block.MerkleRoot.ToHexString());
//     }
//
//     [Test]
//     [Parallelizable]
//     public void TestHashBlockWithVotes()
//     {
//         var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
//
//         var block = Block.Genesis();
//
//         block.AddVote(Vote.TryCreate(Voter.NewVoter(), 0, timestamp)!);
//         block.AddVote(Vote.TryCreate(Voter.NewVoter(), 0, timestamp)!);
//         block.AddVote(Vote.TryCreate(Voter.NewVoter(), 0, timestamp)!);
//         block.AddVote(Vote.TryCreate(Voter.NewVoter(), 0, timestamp)!);
//
//         Console.WriteLine(block.MerkleRoot.ToHexString());
//
//         // mine operation
//         var minedBlock = block.Mine();
//
//         Console.WriteLine(minedBlock.Nonce);
//         Console.WriteLine(minedBlock.Hash.ToHexString());
//
//         Assert.That(minedBlock.IsHashValid, Is.True);
//     }
//
//     [Test]
//     [Parallelizable]
//     public void TestHashBlockWithMaxVotes()
//     {
//         var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
//
//         var block = Block.Genesis();
//
//         block.AddVote(Vote.TryCreate(Voter.NewVoter(), 0, timestamp)!);
//         block.AddVote(Vote.TryCreate(Voter.NewVoter(), 0, timestamp)!);
//         block.AddVote(Vote.TryCreate(Voter.NewVoter(), 0, timestamp)!);
//         block.AddVote(Vote.TryCreate(Voter.NewVoter(), 0, timestamp)!);
//
//         Console.WriteLine(block.MerkleRoot.ToHexString());
//
//         // mine operation
//         var minedBlock = block.Mine();
//
//         Console.WriteLine(minedBlock.Nonce);
//         Console.WriteLine(minedBlock.Hash.ToHexString());
//
//         Assert.That(minedBlock.IsHashValid, Is.True);
//     }
//
//     [Test]
//     [Parallelizable]
//     public void TestCastBackAndForward()
//     {
//         var block = Block.Genesis();
//         for (int i = 0; i < 256; i++)
//             block.AddVote(Vote.TryCreate(Voter.NewVoter(), 0, 0)!);
//
//         block = block.Mine();
//
//         var dto = (BlockDto)block;
//
//         Console.WriteLine(dto);
//
//         var newBlock = (Block)dto;
//
//         Assert.That(block.Hash.ArrayEquals(newBlock.Hash), Is.True);
//         Assert.That(block.MerkleRoot.ArrayEquals(newBlock.MerkleRoot), Is.True);
//         Assert.That(block.PreviousHash.ArrayEquals(newBlock.PreviousHash), Is.True);
//
//         Assert.That(block.Index, Is.EqualTo(newBlock.Index));
//         Assert.That(block.Nonce, Is.EqualTo(newBlock.Nonce));
//
//         Assert.That(block.Votes.Count, Is.EqualTo(newBlock.Votes.Count));
//
//         foreach (var vote in block.Votes)
//         {
//             Console.WriteLine(vote);
//         }
//     }
// }