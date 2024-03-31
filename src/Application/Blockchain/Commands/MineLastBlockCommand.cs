using Application.Blockchain.Events;
using Application.Common.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Blockchain.Commands;

public record MineLastBlockCommand : IRequest<Block>;

// ReSharper disable once UnusedType.Global
public class MineCurrentBlockCommandHandler(
    IAppDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    ILogger<VoteAddedEventHandler> logger)
    : IRequestHandler<MineLastBlockCommand, Block>
{
    public async Task<Block> Handle(MineLastBlockCommand request, CancellationToken ct)
    {
        // fetch all transactions without a blockIndex
        // compile them into a block
        // mine the block.
        // insert it into the database.

        // var currentBlock = await currentBlockAccessor.GetCurrentBlockAsync(ct);
        //
        // currentBlock.Mine();
        //
        // dbContext.Set<Block>().Update(currentBlock);
        //
        // var next = currentBlock.NextBlock();
        // currentBlockAccessor.SetCurrent(next);
        // dbContext.Set<Block>().Add(next);
        //
        // var blockMinedEvent = new BlockMinedEvent(currentBlock.Index);
        // dbContext.AddDomainEvent(blockMinedEvent, dateTimeProvider);
        //
        // await dbContext.SaveChangesAsync(ct);
        //
        // return next;
        throw new NotImplementedException();
    }
}