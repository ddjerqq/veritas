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
internal sealed class VoteAddedEventHandler(IBlockchainEventBroadcast broadcast, IMapper mapper, IMemoryCache cache, IAppDbContext db)
    : INotificationHandler<VoteAddedEvent>
{
    public async Task Handle(VoteAddedEvent notification, CancellationToken ct)
    {
        if (!cache.TryGetValue($"vote_{notification.Hash}", out VoteDto? vote) || vote is not null)
        {
            vote = await db.Set<Vote>()
                .AsNoTracking()
                .Where(v => v.Hash == notification.Hash)
                .ProjectTo<VoteDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(ct);

            cache.Set($"vote_{notification.Hash}", vote, TimeSpan.FromMinutes(10));
        }

        await broadcast.BroadcastVoteAddedEvent(vote!);
    }
}