using Application.Common.Abstractions;
using Application.Dto;
using Microsoft.AspNetCore.SignalR;

namespace Presentation.Hubs;

public class SignalRBlockchainEventBroadcast(IHubContext<BlockchainHub, IBlockchainHubClient> hub) : IBlockchainEventBroadcast
{
    public async Task BroadcastVoteAddedEvent(VoteDto vote)
    {
        await hub.Clients.All.ReceiveVoteAddedEvent(vote);
    }

    public async Task BroadcastBlockAddedEvent(BlockDto block)
    {
        await hub.Clients.All.ReceiveBlockAddedEvent(block);
    }
}