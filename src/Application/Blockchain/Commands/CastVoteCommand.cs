using Application.Common.Abstractions;
using Domain.Entities;
using Domain.Events;
using FluentValidation;
using MediatR;

namespace Application.Blockchain.Commands;

public sealed record CastVoteCommand(string Hash, string Pkey, string Sig, int PartyId, DateTime Timestamp, long Nonce) : IRequest
{
    public Vote GetVote()
    {
        return new Vote
        {
            Hash = Hash,
            Voter = Voter.FromPublicKey(Pkey),
            PartyId = PartyId,
            Timestamp = Timestamp,
            Signature = Sig,
            Nonce = Nonce,
        };
    }
}

// ReSharper disable once UnusedType.Global
public sealed class CastVoteCommandValidator : RequestValidator<CastVoteCommand>
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
            .Length(128)
            .Must(value => value.All(char.IsAsciiHexDigit))
            .WithMessage("Invalid {PropertyName}");

        RuleFor(x => x.PartyId)
            .GreaterThan(0)
            .LessThan(100);

        RuleFor(x => x.Timestamp)
            .Must(ts =>
            {
                var date = dateTimeProvider.UtcNow;

                var offset = date - ts;
                return offset.TotalSeconds is > 0 and < 120;
            })
            .WithMessage("The timestamp must be no more than two minutes old");

        RuleFor(x => x.Nonce)
            .GreaterThan(0)
            .LessThan(500_000_000);

        RuleFor(x => x)
            .Must(command =>
            {
                var currentVoter = currentVoterAccessor.GetCurrentVoter();
                if (currentVoter is null) return false;

                var vote = command.GetVote();

                var voteIsValid = vote is { IsHashValid: true, IsSignatureValid: true };
                var currentVoterIsTheVotesVoter = vote.Voter.Address == currentVoter.Address;

                return voteIsValid && currentVoterIsTheVotesVoter;
            })
            .WithMessage("Invalid vote");
    }
}

// ReSharper disable once UnusedType.Global
public sealed class CastVoteCommandHandler(IAppDbContext dbContext, IDateTimeProvider dateTimeProvider) : IRequestHandler<CastVoteCommand>
{
    public async Task Handle(CastVoteCommand request, CancellationToken ct)
    {
        var vote = request.GetVote();

        dbContext.Set<Vote>().Add(vote);

        var voteAddedEvent = new VoteAddedEvent(vote.Hash);
        dbContext.AddDomainEvent(voteAddedEvent, dateTimeProvider);

        await dbContext.SaveChangesAsync(ct);
    }
}