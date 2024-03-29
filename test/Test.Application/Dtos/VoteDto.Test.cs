using Application;
using Application.Dtos;
using Domain;
using Domain.Common;
using Domain.ValueObjects;

namespace Test.Application.Dtos;

internal class VoteDtoTest
{
    [Test]
    [Parallelizable]
    public void TestConversion()
    {
        var voter = Voter.NewVoter();
        var vote = new Vote(voter, 5, 0);
        vote = vote.Mine();

        var voteDto = (VoteDto)vote;
        Console.WriteLine(voteDto);

        var convertedBack = (Vote)voteDto;

        Assert.That(vote.Hash.ArrayEquals(convertedBack.Hash));
        Assert.That(vote.Voter, Is.EqualTo(convertedBack.Voter));
        Assert.That(vote.PartyId, Is.EqualTo(convertedBack.PartyId));
        Assert.That(vote.Timestamp, Is.EqualTo(convertedBack.Timestamp));
        Assert.That(vote.Signature.ArrayEquals(convertedBack.Signature));
    }

    [Test]
    [Parallelizable]
    public void TestSignatureOnSerializationAndDeserialization()
    {
        var voter = Voter.NewVoter();
        var vote = new Vote(voter, 5, 0);
        vote = vote.Mine();

        var voteDto = (VoteDto)vote;
        Console.WriteLine(voteDto);

        var convertedBack = (Vote)voteDto;

        Assert.That(convertedBack.Voter.Verify(convertedBack.SignaturePayload, vote.Signature), Is.True);
        Assert.That(vote.Voter.Verify(vote.SignaturePayload, convertedBack.Signature), Is.True);
    }
}