using Application.Common.Abstractions;
using Application.Dto;
using Domain.Entities;
using Domain.Events;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Blockchain.Events;

// ReSharper disable once UnusedType.Global
internal sealed class VoteAddedEventHandler(IBlockchainEventBroadcast broadcast, IAppDbContext db) : INotificationHandler<VoteAddedEvent>
{
    public async Task Handle(VoteAddedEvent notification, CancellationToken ct)
    {
        var vote = await db.Set<Vote>()
            .AsNoTracking()
            .Include(v => v.Voter)
            .Where(v => v.Hash == notification.Hash)
            .FirstOrDefaultAsync(ct);

        if (vote is null)
            throw new ValidationException($"Vote with hash: {notification.Hash} not found");

        var dto = new VoteDto(vote.Hash, vote.Nonce, vote.Timestamp, vote.PartyId, vote.VoterAddress, vote.BlockIndex);
        await broadcast.BroadcastVoteAddedEvent(dto);
    }
}