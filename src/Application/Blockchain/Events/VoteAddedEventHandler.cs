using Application.Common.Abstractions;
using Application.Dto;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Events;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Blockchain.Events;

// ReSharper disable once UnusedType.Global
internal sealed class VoteAddedEventHandler(IBlockchainEventBroadcast broadcast, IMapper mapper, IAppDbContext db) : INotificationHandler<VoteAddedEvent>
{
    public async Task Handle(VoteAddedEvent notification, CancellationToken ct)
    {
        var vote = await db.Set<Vote>()
            .AsNoTracking()
            .Where(v => v.Hash == notification.Hash)
            .ProjectTo<VoteDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

        if (vote is null)
            throw new ValidationException($"Vote with hash: {notification.Hash} not found");

        await broadcast.BroadcastVoteAddedEvent(vote);
    }
}