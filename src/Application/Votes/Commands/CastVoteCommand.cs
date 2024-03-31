using Application.Common.Abstractions;
using Application.Votes.Events;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Votes.Commands;

public record CastVoteCommand(string Hash, string Pkey, string Sig, int PartyId, long Timestamp, long Nonce) : IRequest<Vote>
{
    public Vote GetVote()
    {
        return new Vote
        {
            Voter = Voter.FromPublicKey(Pkey),
            PartyId = PartyId,
            Timestamp = Timestamp,
            Signature = Sig,
            Nonce = Nonce,
        };
    }
}

public class CastVoteCommandValidator : RequestValidator<CastVoteCommand>
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

public class CastVoteCommandHandler(
    IAppDbContext dbContext,
    ILogger<VoteAddedEventHandler> logger,
    IMediator mediator,
    ICurrentBlockAccessor currentBlockAccessor) : IRequestHandler<CastVoteCommand, Vote>
{
    public async Task<Vote> Handle(CastVoteCommand request, CancellationToken ct)
    {
        var currentBlock = await currentBlockAccessor.GetCurrentBlockAsync(ct);

        // TODO this must never happen concurrently.
        // have the CastVoteCommand just create and validate the vote,
        // and then push it to the queue to process it synchronously.
        if (currentBlock.Votes.Count >= 256)
        {
            var mineCurrentBlockCommand = new MineCurrentBlockCommand();
            currentBlock = await mediator.Send(mineCurrentBlockCommand, ct);
            // TODO we will no longer need this, after we implement a better, global cache.
            currentBlockAccessor.SetCurrent(currentBlock);
        }

        var vote = request.GetVote();
        currentBlock.TryAddVote(vote);
        dbContext.Set<Vote>().Add(vote);
        await dbContext.SaveChangesAsync(ct);

        // dbContext.ClearChangeTracker();
        dbContext.Set<Block>().Update(currentBlock);
        // var success = dbContext.Set<Block>().TryUpdate(currentBlock);
        // Console.WriteLine(success ? "updating entity success" : "updating entity failed ig?");

        // var voteAddedEvent = new VoteAddedEvent(vote.Hash.ToHexString());
        // dbContext.AddDomainEvent(voteAddedEvent, dateTimeProvider);

        await dbContext.SaveChangesAsync(ct);

        return vote;
    }
}