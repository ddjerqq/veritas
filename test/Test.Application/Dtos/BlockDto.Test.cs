using Application;
using Application.Dtos;
using Domain;
using Domain.Aggregates;
using Domain.Common;
using Domain.ValueObjects;

namespace Test.Application.Dtos;

internal class BlockDtoTest
{
    [Test]
    [Parallelizable]
    public void TestConversion()
    {
        var block = Block.Genesis();

        var blockDto = (BlockDto)block;
        Console.WriteLine(blockDto);

        var convertedBack = (Block)blockDto;

        Assert.That(block.Index, Is.EqualTo(convertedBack.Index));
        Assert.That(block.Index, Is.EqualTo(convertedBack.Index));
        Assert.That(block.Nonce, Is.EqualTo(convertedBack.Nonce));
        Assert.That(block.Hash, Is.EqualTo(convertedBack.Hash));
        Assert.That(block.MerkleRoot, Is.EqualTo(convertedBack.MerkleRoot));
        Assert.That(block.PreviousHash, Is.EqualTo(convertedBack.PreviousHash));

        for (int i = 0; i < block.Votes.Count; i++)
        {
            var vote = block.Votes[i];
            var converted = convertedBack.Votes[i];

            Assert.That(vote.Hash.ArrayEquals(converted.Hash));
            Assert.That(vote.Voter, Is.EqualTo(converted.Voter));
            Assert.That(vote.PartyId, Is.EqualTo(converted.PartyId));
            Assert.That(vote.Timestamp, Is.EqualTo(converted.Timestamp));
            Assert.That(vote.Signature.ArrayEquals(converted.Signature));
        }
    }


    [Test]
    [Parallelizable]
    public void TestConversionOfComplexBlock()
    {
        var block = Block.Genesis().Next();
        for (int i = 0; i < Block.VoteLimit; i++)
        {
            var vote = new Vote(Voter.NewVoter(), 5, 5);
            vote = vote.Mine();
            Assert.That(block.TryAddVote(vote), Is.True);
        }

        block = block.Mine();
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

        for (int i = 0; i < block.Votes.Count; i++)
        {
            var vote = block.Votes[i];
            var converted = convertedBack.Votes[i];

            Assert.That(vote.Hash.ArrayEquals(converted.Hash));
            Assert.That(vote.Voter, Is.EqualTo(converted.Voter));
            Assert.That(vote.PartyId, Is.EqualTo(converted.PartyId));
            Assert.That(vote.Timestamp, Is.EqualTo(converted.Timestamp));
            Assert.That(vote.Signature.ArrayEquals(converted.Signature));
        }
    }
}