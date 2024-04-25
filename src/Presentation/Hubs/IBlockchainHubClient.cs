using Application.Dto;

namespace Presentation.Hubs;

public interface IBlockchainHubClient
{
    public Task ReceiveVoteAddedEvent(VoteDto vote);

    public Task ReceiveBlockAddedEvent(BlockDto block);
}