using Application.Common.Abstractions;
using Application.Dto;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Blockchain.Queries;

public sealed record GetLastNBlockDtosQuery(int Amount) : IRequest<IEnumerable<BlockDto>>;

// TODO REDIS - implement cache eventually.

// ReSharper disable once UnusedType.Global
internal sealed class GetLastNBlockDtosQueryHandler(IAppDbContext dbContext)
    : IRequestHandler<GetLastNBlockDtosQuery, IEnumerable<BlockDto>>
{
    public async Task<IEnumerable<BlockDto>> Handle(GetLastNBlockDtosQuery request, CancellationToken ct)
    {
        // TODO automapper
        var blocks = await dbContext.Set<Block>()
            .AsNoTracking()
            // TODO including these may even be verbose, but it may come in handy in a block list
            .Include(b => b.Votes)
            .ThenInclude(v => v.Voter)
            .OrderBy(x => x.Index)
            .Reverse()
            .Take(request.Amount)
            .ToListAsync(ct);

        return blocks
            .OrderBy(b => b.Index)
            .Select(b => new BlockDto(
                b.Index,
                b.Nonce,
                b.Hash,
                b.MerkleRoot,
                b.PreviousHash,
                b.Votes.LastOrDefault()?.Timestamp ?? DateTime.UnixEpoch,
                b.Votes.Select(v => new VoteDto(v.Hash, v.Nonce, v.Timestamp, v.PartyId, v.VoterAddress, v.BlockIndex)).ToList()
            ));
    }
}