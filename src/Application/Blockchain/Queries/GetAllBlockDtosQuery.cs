using Application.Common.Abstractions;
using Application.Dto;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Blockchain.Queries;

public sealed record GetAllBlockDtosQuery(int Page) : IRequest<IEnumerable<BlockDto>>
{
    public const int PerPage = 20;
}

// ReSharper disable once UnusedType.Global
internal sealed class GetAllBlockDtosQueryHandler(IMapper mapper, IMemoryCache cache, IAppDbContext dbContext)
    : IRequestHandler<GetAllBlockDtosQuery, IEnumerable<BlockDto>>
{
    public async Task<IEnumerable<BlockDto>> Handle(GetAllBlockDtosQuery request, CancellationToken ct)
    {
        if (cache.TryGetValue($"block_all_page_{request.Page}", out ICollection<BlockDto>? blocks) && blocks is { Count: > 0 })
            return blocks;

        blocks = await dbContext.Set<Block>()
            .AsNoTracking()
            .OrderBy(x => x.Index).Reverse()
            .Skip(request.Page * GetAllBlockDtosQuery.PerPage)
            .Take(GetAllBlockDtosQuery.PerPage)
            .ProjectTo<BlockDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);

        cache.Set($"block_all_page_{request.Page}", blocks, TimeSpan.FromSeconds(30));

        return blocks;
    }
}