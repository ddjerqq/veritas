using Domain.Abstractions;
using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Events;

public sealed record VoteAddedEvent(
    string VoterPubKey,
    int PartyId,
    long Timestamp,
    long Nonce,
    string Hash,
    string Signature) : IDomainEvent
{
    public Vote GetVote()
    {
        var voter = Voter.FromPubKey(VoterPubKey.ToBytesFromHex());
        var vote = new Vote
        {
            Voter = voter,
            PartyId = PartyId,
            Timestamp = Timestamp,
            Signature = Signature.ToBytesFromHex(),
            Nonce = Nonce,
        };

        if (vote.Hash.ToHexString() != Hash)
            throw new InvalidOperationException(
                $"failed to convert Vote, the hash does not match. expected: {Hash} was: {vote.Hash.ToHexString()}");

        if (!vote.IsSignatureValid)
            throw new InvalidOperationException(
                $"failed to convert Vote, Invalid signature. was: {vote.Hash.ToHexString()}");

        if (!vote.IsHashValid)
            throw new InvalidOperationException(
                $"failed to convert Vote, the hash is not valid. Missing proof of work. was: {vote.Hash.ToHexString()}");

        return vote;
    }
}
