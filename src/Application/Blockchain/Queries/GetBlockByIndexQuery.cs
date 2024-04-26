using Application.Common.Abstractions;
using Application.Dto;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Blockchain.Queries;

public sealed record GetBlockByIndexQuery(long Index) : IRequest<BlockDto?>;

// TODO REDIS - implement cache eventually.

// ReSharper disable once UnusedType.Global
internal sealed class GetBlockByIndexQueryHandler(IAppDbContext dbContext) : IRequestHandler<GetBlockByIndexQuery, BlockDto?>
{
    public async Task<BlockDto?> Handle(GetBlockByIndexQuery request, CancellationToken ct)
    {
        var block = await dbContext.Set<Block>()
            .AsNoTracking()
            .Include(b => b.Votes)
            .ThenInclude(v => v.Voter)
            .Where(b => b.Index == request.Index)
            .FirstOrDefaultAsync(ct);
        if (block is null) return null;

        var blockDto = new BlockDto(
            block.Index,
            block.Nonce,
            block.Hash,
            block.MerkleRoot,
            block.PreviousHash,
            block.Votes.MaxBy(v => v.Timestamp)?.Timestamp ?? DateTime.UtcNow,
            block.Votes
                .Select(vote => new VoteDto(vote.Hash, vote.Nonce, vote.Timestamp, vote.PartyId, vote.VoterAddress, vote.BlockIndex))
                .ToList());
        return blockDto;
    }
}