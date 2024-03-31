using Application.Dtos;
using Domain.Common;
using Domain.Entities;

namespace Test.Application.Dtos;

internal class BlockDtoTest
{
    [Test]
    [Parallelizable]
    public void TestConversion()
    {
        var block = Block.GenesisBlock();

        var blockDto = (BlockDto)block;
        Console.WriteLine(blockDto);

        var convertedBack = (Block)blockDto;

        Assert.That(block.Index, Is.EqualTo(convertedBack.Index));
        Assert.That(block.Index, Is.EqualTo(convertedBack.Index));
        Assert.That(block.Nonce, Is.EqualTo(convertedBack.Nonce));
        Assert.That(block.Hash, Is.EqualTo(convertedBack.Hash));
        Assert.That(block.MerkleRoot, Is.EqualTo(convertedBack.MerkleRoot));
        Assert.That(block.PreviousHash, Is.EqualTo(convertedBack.PreviousHash));

        var votes = block.Votes.ToList();
        var convertedVotes = convertedBack.Votes.ToList();

        for (var i = 0; i < votes.Count; i++)
        {
            var vote = votes[i];
            var converted = convertedVotes[i];

            Assert.That(vote.Hash.ArrayEquals(converted.Hash));
            Assert.That(vote.Voter, Is.EqualTo(converted.Voter));
            Assert.That(vote.PartyId, Is.EqualTo(converted.PartyId));
            Assert.That(vote.Timestamp, Is.EqualTo(converted.Timestamp));
            Assert.That(vote.Signature, Is.EqualTo(converted.Signature));
        }
    }


    [Test]
    [Parallelizable]
    public void TestConversionOfComplexBlock()
    {
        var block = Block.GenesisBlock().NextBlock();
        for (int i = 0; i < 4; i++)
        {
            var vote = Vote.NewVote(Voter.NewVoter(), 5, 5);
            vote.Mine();
            Assert.That(block.TryAddVote(vote), Is.True);
        }

        block.Mine();
        Console.WriteLine(block.Votes.Count);

        var blockDto = (BlockDto)block;
        Console.WriteLine(blockDto);
        Console.WriteLine(blockDto.Votes.Count);

        var convertedBack = (Block)blockDto;

        Assert.That(block.Index, Is.EqualTo(convertedBack.Index));
        Assert.That(block.Index, Is.EqualTo(convertedBack.Index));
        Assert.That(block.Nonce, Is.EqualTo(convertedBack.Nonce));
        Assert.That(block.Hash, Is.EqualTo(convertedBack.Hash));
        Assert.That(block.MerkleRoot, Is.EqualTo(convertedBack.MerkleRoot));
        Assert.That(block.PreviousHash, Is.EqualTo(convertedBack.PreviousHash));

        var votes = block.Votes.ToList();
        var convertedVotes = convertedBack.Votes.ToList();

        for (int i = 0; i < votes.Count; i++)
        {
            var vote = votes[i];
            var converted = convertedVotes[i];

            Assert.That(vote.Hash.ArrayEquals(converted.Hash));
            Assert.That(vote.Voter, Is.EqualTo(converted.Voter));
            Assert.That(vote.PartyId, Is.EqualTo(converted.PartyId));
            Assert.That(vote.Timestamp, Is.EqualTo(converted.Timestamp));
            Assert.That(vote.Signature, Is.EqualTo(converted.Signature));
        }
    }
}