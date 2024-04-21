using Application.Common.Abstractions;
using Application.Dto;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Blockchain.Queries;

public sealed record GetVoteByHashQuery(string Hash) : IRequest<VoteDto?>;

// TODO REDIS - implement cache eventually.

// ReSharper disable once UnusedType.Global
internal sealed class GetVoteByHashQueryHandler(IAppDbContext dbContext) : IRequestHandler<GetVoteByHashQuery, VoteDto?>
{
    public async Task<VoteDto?> Handle(GetVoteByHashQuery request, CancellationToken ct)
    {
        return await dbContext.Set<Vote>()
            .AsNoTracking()
            .Select(vote => new VoteDto(vote.Hash, vote.Nonce, vote.Timestamp, vote.PartyId, vote.VoterAddress, vote.BlockIndex))
            .FirstOrDefaultAsync(ct);
    }
}