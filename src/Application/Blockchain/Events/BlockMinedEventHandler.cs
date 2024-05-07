using Application.Common.Abstractions;
using Application.Dto;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Blockchain.Events;

// ReSharper disable once UnusedType.Global
internal sealed class BlockMinedEventHandler(IBlockchainEventBroadcast eventBroadcast, IMapper mapper, IMemoryCache cache, IAppDbContext db) : INotificationHandler<BlockMinedEvent>
{
    public async Task Handle(BlockMinedEvent notification, CancellationToken ct)
    {
        if (!cache.TryGetValue($"block_{notification.BlockIndex}", out BlockDto? block) || block is not null)
        {
            block = await db.Set<Block>()
                .AsNoTracking()
                .Where(b => b.Index == notification.BlockIndex)
                .ProjectTo<BlockDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(ct);

            cache.Set($"block_{notification.BlockIndex}", block, TimeSpan.FromMinutes(10));
        }

        await eventBroadcast.BroadcastBlockAddedEvent(block!);
    }
}