using Application;
using Application.Dtos;
using AutoMapper;
using Domain;
using Domain.Aggregates;
using Domain.Common;

namespace Test.Application.Dtos;

internal class BlockDtoTest
{
    private IMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(DomainAssembly.Assembly);
            cfg.AddMaps(ApplicationAssembly.Assembly);
        });
        _mapper = new Mapper(config);
    }

    [Test]
    public void TestConversion()
    {
        var block = Block.Genesis();

        var blockDto = _mapper.Map<Block, BlockDto>(block);
        Console.WriteLine(blockDto);

        var convertedBack = _mapper.Map<BlockDto, Block>(blockDto);

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