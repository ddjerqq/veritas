using System.ComponentModel.DataAnnotations;
using Application.Common.Abstractions;
using Application.Dto;
using Domain.Entities;
using Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Blockchain.Events;

// ReSharper disable once UnusedType.Global
internal sealed class BlockMinedEventHandler(IBlockchainEventBroadcast eventBroadcast, IAppDbContext db) : INotificationHandler<BlockMinedEvent>
{
    public async Task Handle(BlockMinedEvent notification, CancellationToken ct)
    {
        var block = await db.Set<Block>()
            .AsNoTracking()
            .Include(b => b.Votes)
            .ThenInclude(v => v.Voter)
            .Where(b => b.Index == notification.BlockIndex)
            .FirstOrDefaultAsync(ct);

        if (block is null)
            throw new ValidationException($"Block with index: {notification.BlockIndex} not found");

        var dto = new BlockDto(
            block.Index,
            block.Nonce,
            block.Hash,
            block.MerkleRoot,
            block.PreviousHash,
            block.Votes.MaxBy(v => v.Timestamp)?.Timestamp ?? DateTime.UtcNow,
            block.Votes.Select(v => new VoteDto(v.Hash, v.Nonce, v.Timestamp, v.PartyId, v.VoterAddress, v.BlockIndex)).ToList());

        await eventBroadcast.BroadcastBlockAddedEvent(dto);
    }
}