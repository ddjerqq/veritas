using Application.Common.Abstractions;
using Application.Dto;
using Microsoft.AspNetCore.SignalR;

namespace Presentation.Hubs;

public class BlockchainHub : Hub<IBlockchainHubClient>, IBlockchainEventBroadcast
{
    public async Task BroadcastVoteAddedEvent(VoteDto vote)
    {
        await Clients.All.ReceiveVoteAddedEvent(vote);
    }

    public async Task BroadcastBlockAddedEvent(BlockDto block)
    {
        await Clients.All.ReceiveBlockAddedEvent(block);
    }
}