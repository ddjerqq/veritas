using Application.Common.Abstractions;
using Application.Dto;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Blockchain.Queries;

public sealed record GetAllBlockDtosQuery(int Page) : IRequest<IEnumerable<BlockDto>>
{
    public const int PerPage = 20;
}

// TODO REDIS - implement cache eventually.

// ReSharper disable once UnusedType.Global
internal sealed class GetAllBlockDtosQueryHandler(IMapper mapper, IAppDbContext dbContext)
    : IRequestHandler<GetAllBlockDtosQuery, IEnumerable<BlockDto>>
{
    public async Task<IEnumerable<BlockDto>> Handle(GetAllBlockDtosQuery request, CancellationToken ct)
    {
        var blocks = await dbContext.Set<Block>()
            .AsNoTracking()
            .OrderBy(x => x.Index).Reverse()
            .Skip(request.Page * GetAllBlockDtosQuery.PerPage)
            .Take(GetAllBlockDtosQuery.PerPage)
            .ProjectTo<BlockDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return blocks;
    }
}