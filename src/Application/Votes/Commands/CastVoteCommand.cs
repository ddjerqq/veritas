using Application.Abstractions;
using Application.Dtos;
using Application.Votes.Events;
using Domain.Common;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Votes.Commands;

public record CastVoteCommand(string Hash, string Pkey, string Sig, int PartyId, long Timestamp, long Nonce) : IRequest<Vote>
{
    public Vote GetVote()
    {
        var votesVoter = Voter.FromPubKey(Pkey.ToBytesFromHex());

        return new Vote
        {
            Voter = votesVoter,
            PartyId = PartyId,
            Timestamp = Timestamp,
            Signature = Sig.ToBytesFromHex(),
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
    IBlockCache blockCache) : IRequestHandler<CastVoteCommand, Vote>
{
    public async Task<Vote> Handle(CastVoteCommand request, CancellationToken ct)
    {
        var currentBlockDto = await blockCache.GetCurrentAsync(ct);
        var currentBlock = (Block)currentBlockDto;

        // TODO this must never happen concurrently.
        // have the CastVoteCommand just create and validate the vote,
        // and then push it to the queue to process it synchronously.
        if (currentBlock.Votes.Count >= Block.VoteLimit)
        {
            var mineCurrentBlockCommand = new MineCurrentBlockCommand();
            currentBlock = await mediator.Send(mineCurrentBlockCommand, ct);
            currentBlockDto = currentBlock;
            // TODO we will no longer need this, after we implement a better, global cache.
            blockCache.SetCurrent(currentBlockDto);
        }

        var vote = request.GetVote();
        currentBlock.TryAddVote(vote);
        dbContext.Set<VoteDto>().Add(vote);
        await dbContext.SaveChangesAsync(ct);

        currentBlockDto.CopyFrom(currentBlock);
        dbContext.ClearChangeTracker();
        dbContext.Set<BlockDto>().Update(currentBlockDto);
        // var success = dbContext.Set<BlockDto>().TryUpdate(currentBlockDto);
        // Console.WriteLine(success ? "updating entity success" : "updating entity failed ig?");

        // var voteAddedEvent = new VoteAddedEvent(vote.Hash.ToHexString());
        // dbContext.AddDomainEvent(voteAddedEvent, dateTimeProvider);

        await dbContext.SaveChangesAsync(ct);

        return vote;
    }
}