using Application.Blockchain.Queries;
using Application.Common.Abstractions;
using Domain.Common;
using Domain.Entities;
using Domain.Events;
using FluentValidation;
using MediatR;

namespace Application.Blockchain.Commands;

public sealed record CastVoteCommand(string Hash, string PublicKey, string PrivateKey, int PartyId, long Timestamp, long Nonce) : IRequest
{
    private Voter? _voter;
    private string? _sig;

    public Vote GetVote()
    {
        // so that the signature does not change during access inside validation.
        var voter = _voter ??= Voter.FromKeyPair(PublicKey, PrivateKey);
        var signature = _sig ??= voter.Sign(VoteExt.GetSignaturePayload(PartyId, Timestamp)).ToHexString();

        return new Vote
        {
            Hash = Hash,
            Voter = voter,
            PartyId = PartyId,
            Timestamp = Timestamp.ToUtcDateTime(),
            Signature = signature,
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

        RuleFor(x => x.PublicKey)
            .Length(64, 256)
            .Must(value => value.All(char.IsAsciiHexDigit))
            .WithMessage("Invalid {PropertyName}");

        RuleFor(x => x.PartyId)
            .GreaterThan(0)
            .LessThan(100);

        RuleFor(x => x.Timestamp)
            .Must(ts =>
            {
                var date = dateTimeProvider.UtcNow;
                var offset = date - ts.ToUtcDateTime();
                return offset is { Ticks: > 0, TotalMinutes: < 5 };
            })
            .WithMessage("The timestamp must be no more than five minutes old");

        RuleFor(x => x.Nonce)
            .GreaterThan(0);

        RuleFor(x => x)
            .Must(command =>
            {
                var currentVoter = currentVoterAccessor.TryGetCurrentVoter();
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
internal sealed class CastVoteCommandHandler(
    IAppDbContext dbContext,
    ISender mediator,
    ICurrentVoterAccessor currentVoterAccessor,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<CastVoteCommand>
{
    public async Task Handle(CastVoteCommand request, CancellationToken ct)
    {
        var voter = currentVoterAccessor.GetCurrentVoter();
        var voterInfoQuery = new GetVoterByAddressQuery(voter.Address);
        var voterInfo = await mediator.Send(voterInfoQuery, ct);

        var lastVote = voterInfo?.Votes.LastOrDefault();
        if (lastVote is not null && dateTimeProvider.UtcNow - lastVote.Timestamp < TimeSpan.FromHours(12))
            throw new ValidationException("User tried voting more than once in a 12 hour window.");

        var vote = request.GetVote();

        dbContext.Set<Vote>().Add(vote);

        var voteAddedEvent = new VoteAddedEvent(vote.Hash);
        dbContext.AddDomainEvent(voteAddedEvent, dateTimeProvider);

        await dbContext.SaveChangesAsync(ct);
    }
}