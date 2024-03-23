using System.ComponentModel;
using Api.Common;
using Api.Data.Dto;
using Api.Services;
using Api.Services.Interfaces;
using NUnit.Framework;

namespace Api.Data.Models;

[EditorBrowsable(EditorBrowsableState.Never)]
internal class VoteTest
{
    private IDateTimeProvider _time = new TestDateTimeProvider(new DateTime(2024, 01, 01));

    [Test]
    public void TestConversion()
    {
        var voter = Voter.NewVoter();
        var vote = Vote.TryCreate(voter, 5, _time.UtcNowUnixTimeMilliseconds, 0);

        var voteDto = (VoteDto)vote;
        Console.WriteLine(voteDto);

        var convertedBack = (Vote)voteDto;
        Assert.That(vote.Hash.ArrayEquals(convertedBack.Hash));
        Assert.That(vote.Voter, Is.EqualTo(convertedBack.Voter));
        Assert.That(vote.PartyId, Is.EqualTo(convertedBack.PartyId));
        Assert.That(vote.Timestamp, Is.EqualTo(convertedBack.Timestamp));
        Assert.That(vote.BlockIndex, Is.EqualTo(convertedBack.BlockIndex));
        Assert.That(vote.Signature.ArrayEquals(convertedBack.Signature));
    }

    [Test]
    public void TestSignatureOnSerializationAndDeserialization()
    {
        var voter = Voter.NewVoter();
        var vote = Vote.TryCreate(voter, 0, _time.UtcNowUnixTimeMilliseconds, 0);

        var voteDto = (VoteDto)vote;
        var deserializedVote = (Vote)voteDto;

        Assert.That(deserializedVote.VerifySignature(vote.Signature), Is.True);
    }

    [Test]
    [Parallelizable]
    public void TestVoteHash()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var vote = Vote.TryCreate(Voter.NewVoter(), 0, timestamp, 0);
        var hash = vote.Hash.ToHexString();

        Console.WriteLine(hash);
        Assert.That(hash, Is.Not.Null);
    }

    [Test]
    [Parallelizable]
    public void TestVoteSignature()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var vote = Vote.TryCreate(Voter.NewVoter(), 0, timestamp, 0);

        Console.WriteLine($"Vote Hash: {vote.Hash.ToHexString()}");
        Assert.That(vote.Hash.ToHexString(), Is.Not.Null);

        Console.WriteLine($"Vote Signature: {vote.Signature.ToBase64String()}");
        Assert.That(vote.Signature.ToBase64String(), Is.Not.Null);

        var isValid = vote.VerifySignature(vote.Signature);
        Assert.That(isValid, Is.True);
    }

    [Test]
    [Parallelizable]
    public void TestThrowsOnInvalidHashAndSignature()
    {
        var vote = Vote.TryCreate(Voter.NewVoter(), 0, 0, 0)!;
        var dto = (VoteDto)vote;

        var badHash = dto with { Hash = new string('0', 64) };
        Assert.Throws<InvalidOperationException>(() => _ = (Vote)badHash);

        var badSig = dto with { Signature = new string('0', 64) };
        Assert.Throws<InvalidOperationException>(() => _ = (Vote)badSig);
    }
}