using System.Diagnostics;
using Application.Abstractions;
using Application.Dtos;
using Application.Votes.Events;
using Domain.Aggregates;
using Domain.Common;
using Domain.Events;
using Domain.ValueObjects;
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

public class VoteCommandHandler(
    IAppDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    ILogger<VoteAddedEventHandler> logger,
    IBlockCache blockCache
)
    : IRequestHandler<CastVoteCommand, Vote>
{
    public async Task<Vote> Handle(CastVoteCommand request, CancellationToken ct)
    {
        var currentBlock = await blockCache.GetCurrentAsync(ct);

        if (currentBlock.Votes.Count >= Block.VoteLimit)
        {
            currentBlock = await MineBlockAsync(ct);
        }

        var vote = request.GetVote();
        vote.BlockIndex = currentBlock.Index;

        currentBlock.TryAddVote(vote);

        dbContext.Set<VoteDto>().Add(vote);
        await dbContext.SaveChangesAsync(ct);

        await PublishVoteAddedEventAsync(vote, ct);

        return vote;
    }


    private async Task<Block> MineBlockAsync(CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();

        var currentBlock = await blockCache.MineAndRotateAsync(ct);

        stopwatch.Stop();
        logger.LogInformation("new block mined in {Elapsed:c}", stopwatch.Elapsed);

        dbContext.Set<BlockDto>().Add(currentBlock);

        var blockMinedEvent = new BlockMinedEvent(currentBlock.Index);
        dbContext.AddDomainEvent(blockMinedEvent, dateTimeProvider);

        await dbContext.SaveChangesAsync(ct);

        return currentBlock;
    }

    private async Task PublishVoteAddedEventAsync(Vote vote, CancellationToken ct = default)
    {
        var voteAddedEvent = new VoteAddedEvent(vote.Hash.ToHexString());
        dbContext.AddDomainEvent(voteAddedEvent, dateTimeProvider);
        await dbContext.SaveChangesAsync(ct);
    }
}