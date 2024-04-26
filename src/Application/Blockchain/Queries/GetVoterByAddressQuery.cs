using Application.Common.Abstractions;
using Application.Dto;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Blockchain.Queries;

public sealed record GetVoterByAddressQuery(string Address) : IRequest<VoterDto?>;

// ReSharper disable once UnusedType.Global
internal sealed class GetVoterInfoQueryHandler(IAppDbContext dbContext) : IRequestHandler<GetVoterByAddressQuery, VoterDto?>
{
    public async Task<VoterDto?> Handle(GetVoterByAddressQuery request, CancellationToken ct)
    {
        var voter = await dbContext.Set<Voter>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Address == request.Address, ct);

        if (voter is null) return null;

        var votes = await dbContext.Set<Vote>()
            .AsNoTracking()
            .Where(x => x.Voter.Address == request.Address)
            .Select(vote => new VoteDto(vote.Hash, vote.Nonce, vote.Timestamp, vote.PartyId, vote.VoterAddress, vote.BlockIndex))
            .ToListAsync(ct);

        return new VoterDto(voter.Address, voter.PublicKey, votes);
    }
}