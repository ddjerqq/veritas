using Application.Common.Abstractions;
using Application.Dto;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Blockchain.Queries;

public sealed record GetVoteByHashQuery(string Hash) : IRequest<VoteDto?>;

// ReSharper disable once UnusedType.Global
internal sealed class GetVoteByHashQueryHandler(IMapper mapper, IMemoryCache cache, IAppDbContext dbContext) : IRequestHandler<GetVoteByHashQuery, VoteDto?>
{
    public async Task<VoteDto?> Handle(GetVoteByHashQuery request, CancellationToken ct)
    {
        if (cache.TryGetValue($"vote_{request.Hash}", out VoteDto? vote) && vote is not null)
            return vote;

        vote = await dbContext.Set<Vote>()
            .AsNoTracking()
            .Where(v => v.Hash == request.Hash)
            .ProjectTo<VoteDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

        cache.Set($"vote_{request.Hash}", vote, TimeSpan.FromMinutes(10));

        return vote;
    }
}