using Application.Common.Abstractions;
using Application.Dto;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Blockchain.Queries;

public sealed record GetBlockByIndexQuery(long Index) : IRequest<BlockDto?>;

// ReSharper disable once UnusedType.Global
internal sealed class GetBlockByIndexQueryHandler(IMapper mapper, IMemoryCache cache, IAppDbContext dbContext)
    : IRequestHandler<GetBlockByIndexQuery, BlockDto?>
{
    public async Task<BlockDto?> Handle(GetBlockByIndexQuery request, CancellationToken ct)
    {
        if (cache.TryGetValue($"block_{request.Index}", out BlockDto? block) && block is not null)
            return block;

        block = await dbContext.Set<Block>()
            .AsNoTracking()
            .Where(b => b.Index == request.Index)
            .ProjectTo<BlockDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

        cache.Set($"block_{request.Index}", block, TimeSpan.FromMinutes(10));

        return block;
    }
}