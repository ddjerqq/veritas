using Application.Dto;

namespace Application.Common.Abstractions;

public interface IBlockchainEventBroadcast
{
    public Task BroadcastVoteAddedEvent(VoteDto vote);

    public Task BroadcastBlockAddedEvent(BlockDto block);
}