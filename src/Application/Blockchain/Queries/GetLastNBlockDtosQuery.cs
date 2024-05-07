using Application.Common.Abstractions;
using Application.Dto;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Blockchain.Queries;

public sealed record GetLastNBlockDtosQuery(int Amount) : IRequest<IEnumerable<BlockDto>>;

// ReSharper disable once UnusedType.Global
internal sealed class GetLastNBlockDtosQueryHandler(IMapper mapper, IMemoryCache cache, IAppDbContext dbContext)
    : IRequestHandler<GetLastNBlockDtosQuery, IEnumerable<BlockDto>>
{
    public async Task<IEnumerable<BlockDto>> Handle(GetLastNBlockDtosQuery request, CancellationToken ct)
    {
        if (cache.TryGetValue($"block_last_{request.Amount}", out ICollection<BlockDto>? blocks) && blocks is { Count: > 0 })
            return blocks;

        blocks = await dbContext.Set<Block>()
            .AsNoTracking()
            .OrderBy(x => x.Index).Reverse()
            .Take(request.Amount).Reverse()
            .ProjectTo<BlockDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);

        cache.Set($"block_last_{request.Amount}", blocks, TimeSpan.FromSeconds(30));

        return blocks;
    }
}