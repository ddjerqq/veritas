using System.ComponentModel.DataAnnotations;
using Application.Common.Abstractions;
using Application.Dto;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Blockchain.Events;

// ReSharper disable once UnusedType.Global
internal sealed class BlockMinedEventHandler(IBlockchainEventBroadcast eventBroadcast, IMapper mapper, IAppDbContext db) : INotificationHandler<BlockMinedEvent>
{
    public async Task Handle(BlockMinedEvent notification, CancellationToken ct)
    {
        var block = await db.Set<Block>()
            .AsNoTracking()
            .Where(b => b.Index == notification.BlockIndex)
            .ProjectTo<BlockDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

        if (block is null)
            throw new ValidationException($"Block with index: {notification.BlockIndex} not found");

        await eventBroadcast.BroadcastBlockAddedEvent(block);
    }
}