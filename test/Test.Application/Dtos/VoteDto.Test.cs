using Application;
using Application.Dtos;
using AutoMapper;
using Domain;
using Domain.Common;
using Domain.ValueObjects;

namespace Test.Application.Dtos;

internal class VoteDtoTest
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
        var voter = Voter.NewVoter();
        var vote = new Vote(voter, 5, 0);

        var voteDto = _mapper.Map<Vote, VoteDto>(vote);
        Console.WriteLine(voteDto);

        var convertedBack = _mapper.Map<VoteDto, Vote>(voteDto);

        Assert.That(vote.Hash.ArrayEquals(convertedBack.Hash));
        Assert.That(vote.Voter, Is.EqualTo(convertedBack.Voter));
        Assert.That(vote.PartyId, Is.EqualTo(convertedBack.PartyId));
        Assert.That(vote.Timestamp, Is.EqualTo(convertedBack.Timestamp));
        Assert.That(vote.Signature.ArrayEquals(convertedBack.Signature));
    }

    [Test]
    public void TestSignatureOnSerializationAndDeserialization()
    {
        var voter = Voter.NewVoter();
        var vote = new Vote(voter, 5, 0);

        var voteDto = _mapper.Map<Vote, VoteDto>(vote);
        Console.WriteLine(voteDto);

        var convertedBack = _mapper.Map<VoteDto, Vote>(voteDto);

        Assert.That(convertedBack.Voter.Verify(convertedBack.SignaturePayload, vote.Signature), Is.True);
        Assert.That(vote.Voter.Verify(vote.SignaturePayload, convertedBack.Signature), Is.True);
    }
}