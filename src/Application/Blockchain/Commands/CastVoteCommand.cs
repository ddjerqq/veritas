using Application.Common.Abstractions;
using Domain.Common;
using Domain.Entities;
using Domain.Events;
using Domain.ValueObjects;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
            VoterAddress = voter.Address,
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
    public CastVoteCommandValidator(IAppDbContext dbContext, ICurrentVoterAccessor currentVoterAccessor, IDateTimeProvider dateTimeProvider)
    {
        RuleFor(x => x.Hash)
            .NotEmpty()
            .Length(64)
            .Must(value => value.All(char.IsAsciiHexDigit))
            .WithMessage("Invalid {PropertyName}");

        RuleFor(x => x.PublicKey)
            .NotEmpty()
            .Length(64, 256)
            .Must(value => value.All(char.IsAsciiHexDigit))
            .WithMessage("Invalid {PropertyName}");

        RuleFor(x => x.PartyId)
            .GreaterThan(0)
            .Must(id => Party.Allowed.Contains(id))
            .WithMessage("Party Id is not allowed");

        RuleFor(x => x.Timestamp)
            .Must(ts =>
            {
                // TODO investigate the issue with voting here.
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

        // this cant be validated with ASP.NET core's validation pipeline, so we must put it in a ruleset
        // because MediatR RequestValidationBehavior validates all rulesets, as configured.
        RuleSet("async", () =>
        {
            RuleFor(x => x)
                .MustAsync(async (_, ct) =>
                {
                    var currentVoter = currentVoterAccessor.GetCurrentVoter();
                    var lastVoteTime = await dbContext.Set<Vote>()
                        .AsNoTracking()
                        .Where(v => v.VoterAddress == currentVoter.Address)
                        .Select(v => v.Timestamp)
                        .Order()
                        .LastOrDefaultAsync(ct);

                    // the user has not voted yet
                    if (lastVoteTime == DateTime.MinValue)
                        return true;

                    var diff = lastVoteTime - dateTimeProvider.UtcNow;
                    return diff < Vote.VotePer;
                })
                .WithMessage("User tried voting more than once in the allowed time window");
        });
    }
}

// ReSharper disable once UnusedType.Global
internal sealed class CastVoteCommandHandler(IAppDbContext dbContext, IDateTimeProvider dateTimeProvider) : IRequestHandler<CastVoteCommand>
{
    public async Task Handle(CastVoteCommand request, CancellationToken ct)
    {
        var vote = request.GetVote();

        // if voter exists in database already, do not touch it
        var voterCount = await dbContext.Set<Voter>()
            .Where(v => v.Address == vote.VoterAddress)
            .CountAsync(ct);

        if (voterCount == 1)
            dbContext.Set<Voter>().Entry(vote.Voter).State = EntityState.Unchanged;

        dbContext.Set<Vote>().Add(vote);

        var voteAddedEvent = new VoteAddedEvent(vote.Hash);
        dbContext.AddDomainEvent(voteAddedEvent, dateTimeProvider);

        await dbContext.SaveChangesAsync(ct);
    }
}