using Application.Common.Abstractions;
using Application.Dto;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Blockchain.Queries;

public sealed record GetVoterByAddressQuery(string Address) : IRequest<VoterDto?>;

// ReSharper disable once UnusedType.Global
internal sealed class GetVoterInfoQueryHandler(IMapper mapper, IAppDbContext dbContext) : IRequestHandler<GetVoterByAddressQuery, VoterDto?>
{
    public async Task<VoterDto?> Handle(GetVoterByAddressQuery request, CancellationToken ct)
    {
        var voter = await dbContext.Set<Voter>()
            .AsNoTracking()
            .Where(x => x.Address == request.Address)
            .ProjectTo<VoterDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

        return voter;
    }
}