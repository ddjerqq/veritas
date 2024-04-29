using Application;
using Application.Dto;
using AutoMapper;
using Domain.Entities;

namespace Test.Application.Mapping;

public class MappingTest
{
    private IMapper _mapper;

    [SetUp]
    public void Setup()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(ApplicationAssembly.Assembly);
        });
        _mapper = new Mapper(config);
    }

    [Test]
    [NonParallelizable]
    public void TestVoterDtoMappingIsWorkingCorrectly()
    {
        var voter = Voter.NewVoter();
        var vote = Vote.NewVote(voter, 5, DateTime.UtcNow);
        vote.Mine();

        voter.Votes.Add(vote);

        var voterDto = _mapper.Map<VoterDto>(voter);
        Console.WriteLine(voterDto);

        var voteDto = voterDto.Votes.First();
        Console.WriteLine(voteDto);

        Assert.Multiple(() =>
        {
            Assert.That(voter.Address, Is.EqualTo(voterDto.Address));
            Assert.That(voter.PublicKey, Is.EqualTo(voterDto.PublicKey));
            Assert.That(voter.Votes, Has.Count.EqualTo(voterDto.Votes.Count));

            Assert.That(voteDto, Is.Not.Null);
            Assert.That(vote.Hash, Is.EqualTo(voteDto.Hash));
            Assert.That(vote.Nonce, Is.EqualTo(voteDto.Nonce));
            Assert.That(vote.Timestamp, Is.EqualTo(voteDto.Timestamp));
            Assert.That(vote.PartyId, Is.EqualTo(voteDto.PartyId));
            Assert.That(vote.Voter.Address, Is.EqualTo(voteDto.VoterAddress));
            Assert.That(vote.BlockIndex, Is.EqualTo(voteDto.BlockIndex));
        });
    }

    [Test]
    [Parallelizable]
    public void TestVoteDtoMappingIsWorkingCorrectly()
    {
        var voter = Voter.NewVoter();
        var vote = Vote.NewVote(voter, 5, DateTime.UtcNow);
        var dto = _mapper.Map<VoteDto>(vote);

        Console.WriteLine(dto);

        Assert.Multiple(() =>
        {
            Assert.That(vote.Hash, Is.EqualTo(dto.Hash));
            Assert.That(vote.Nonce, Is.EqualTo(dto.Nonce));
            Assert.That(vote.Timestamp, Is.EqualTo(dto.Timestamp));
            Assert.That(vote.PartyId, Is.EqualTo(dto.PartyId));
            Assert.That(vote.Voter.Address, Is.EqualTo(dto.VoterAddress));
            Assert.That(vote.BlockIndex, Is.EqualTo(dto.BlockIndex));
        });
    }

    [Test]
    [NonParallelizable]
    public void TestBlockDtoMappingIsWorkingCorrectly()
    {
        var block = Block.GenesisBlock().NextBlock();

        var voter = Voter.NewVoter();
        var vote = Vote.NewVote(voter, 5, DateTime.UtcNow);
        vote.Mine();

        block.AddVote(vote);
        block.Mine();

        var dto = _mapper.Map<BlockDto>(block);

        Console.WriteLine(dto);

        Assert.Multiple(() =>
        {
            Assert.That(block.Index, Is.EqualTo(dto.Index));
            Assert.That(block.Hash, Is.EqualTo(dto.Hash));
            Assert.That(block.PreviousHash, Is.EqualTo(dto.PreviousHash));
            Assert.That(block.Nonce, Is.EqualTo(dto.Nonce));
            Assert.That(block.MerkleRoot, Is.EqualTo(dto.MerkleRoot));
        });
    }
}