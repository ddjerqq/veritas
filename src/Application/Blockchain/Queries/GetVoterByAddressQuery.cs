using Application.Common.Abstractions;
using Application.Dto;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Blockchain.Queries;

public sealed record GetVoterByAddressQuery(string Address) : IRequest<VoterDto?>;

// ReSharper disable once UnusedType.Global
internal sealed class GetVoterInfoQueryHandler(IMapper mapper, IMemoryCache cache, IAppDbContext dbContext) : IRequestHandler<GetVoterByAddressQuery, VoterDto?>
{
    public async Task<VoterDto?> Handle(GetVoterByAddressQuery request, CancellationToken ct)
    {
        if (cache.TryGetValue($"voter_{request.Address}", out VoterDto? voter) && voter is not null)
            return voter;

        voter = await dbContext.Set<Voter>()
            .AsNoTracking()
            .Where(x => x.Address == request.Address)
            .ProjectTo<VoterDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

        cache.Set($"voter_{request.Address}", voter, TimeSpan.FromMinutes(10));

        return voter;
    }
}