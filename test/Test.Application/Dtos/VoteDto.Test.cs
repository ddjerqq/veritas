using Application.Dtos;
using Domain.Common;
using Domain.Entities;

namespace Test.Application.Dtos;

internal class VoteDtoTest
{
    [Test]
    [Parallelizable]
    public void TestConversion()
    {
        var voter = Voter.NewVoter();
        var vote = Vote.NewVote(voter, 5, 0);
        vote.Mine();

        var voteDto = (VoteDto)vote;
        Console.WriteLine(voteDto);

        var convertedBack = (Vote)voteDto;

        Assert.That(vote.Hash.ArrayEquals(convertedBack.Hash));
        Assert.That(vote.Voter, Is.EqualTo(convertedBack.Voter));
        Assert.That(vote.PartyId, Is.EqualTo(convertedBack.PartyId));
        Assert.That(vote.Timestamp, Is.EqualTo(convertedBack.Timestamp));
        Assert.That(vote.Signature, Is.EqualTo(convertedBack.Signature));
    }

    [Test]
    [Parallelizable]
    public void TestSignatureOnSerializationAndDeserialization()
    {
        var voter = Voter.NewVoter();
        var vote = Vote.NewVote(voter, 5, 0);
        vote.Mine();

        var voteDto = (VoteDto)vote;
        Console.WriteLine(voteDto);

        var convertedBack = (Vote)voteDto;

        Assert.That(convertedBack.Voter.Verify(convertedBack.GetSignaturePayload(), vote.Signature.ToBytesFromHex()), Is.True);
        Assert.That(vote.Voter.Verify(vote.GetSignaturePayload(), convertedBack.Signature.ToBytesFromHex()), Is.True);
    }
}