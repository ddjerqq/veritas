using Application.Abstractions;
using Application.Common;
using Domain.Common;
using Domain.Events;
using Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Application.Votes.Commands;

public record CastVoteCommand(string Hash, string Pkey, string Sig, int PartyId, long Timestamp, long Nonce) : IRequest<Vote>;

public class CastVoteCommandValidator : AbstractValidator<CastVoteCommand>
{
    public CastVoteCommandValidator(ICurrentVoterAccessor currentVoterAccessor, IDateTimeProvider dateTimeProvider)
    {
        RuleFor(x => x.Hash)
            .Length(64)
            .Must(value => value.All(char.IsAsciiHexDigit))
            .WithMessage("Invalid {PropertyName}");

        RuleFor(x => x.Pkey)
            .Length(64, 256)
            .Must(value => value.All(char.IsAsciiHexDigit))
            .WithMessage("Invalid {PropertyName}");

        RuleFor(x => x.Sig)
            .Length(64, 256)
            .Must(value => value.All(char.IsAsciiHexDigit))
            .WithMessage("Invalid {PropertyName}");

        RuleFor(x => x.PartyId)
            .GreaterThan(0)
            .LessThan(100);

        RuleFor(x => x.Timestamp)
            .Must(ts =>
            {
                var dateProduced = DateTimeOffset.FromUnixTimeMilliseconds(ts).UtcDateTime;
                var date = dateTimeProvider.UtcNow;

                var offset = date - dateProduced;
                return offset.TotalSeconds is > 0 and < 120;
            })
            .WithMessage("The timestamp must be no more than two minutes old");

        RuleFor(x => x.Nonce)
            .GreaterThan(0)
            .LessThan(500_000_000);

        RuleFor(x => x)
            .Must(voteDto =>
            {
                var currentVoter = currentVoterAccessor.GetCurrentVoter();
                if (currentVoter is null) return false;

                var votesVoter = Voter.FromPubKey(voteDto.Pkey.ToBytesFromHex());
                var vote = new Vote
                {
                    Voter = votesVoter,
                    PartyId = voteDto.PartyId,
                    Timestamp = voteDto.Timestamp,
                    Signature = voteDto.Sig.ToBytesFromHex(),
                    Nonce = voteDto.Nonce,
                };

                var voteIsValid = vote is { IsHashValid: true, IsSignatureValid: true };
                var currentVoterIsTheVotesVoter = vote.Voter.Address == currentVoter.Address;

                return voteIsValid && currentVoterIsTheVotesVoter;
            })
            .WithMessage("Invalid vote");
    }
}

public class VoteCommandHandler(
    IAppDbContext dbContext,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<CastVoteCommand, Vote>
{
    public async Task<Vote> Handle(CastVoteCommand request, CancellationToken ct)
    {
        var votesVoter = Voter.FromPubKey(request.Pkey.ToBytesFromHex());
        var vote = new Vote
        {
            Voter = votesVoter,
            PartyId = request.PartyId,
            Timestamp = request.Timestamp,
            Signature = request.Sig.ToBytesFromHex(),
            Nonce = request.Nonce,
        };

        var ev = new VoteAddedEvent(
            vote.Voter.PublicKey.ToHexString(),
            vote.PartyId,
            vote.Timestamp,
            vote.Nonce,
            vote.Hash.ToHexString(),
            vote.Signature.ToHexString());

        var msg = OutboxMessage.FromDomainEvent(ev, dateTimeProvider);
        dbContext.Set<OutboxMessage>().Add(msg);
        await dbContext.SaveChangesAsync(ct);

        return vote;
    }
}