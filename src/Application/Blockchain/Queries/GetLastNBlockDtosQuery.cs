using Application.Common.Abstractions;
using Application.Dto;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Blockchain.Queries;

public sealed record GetLastNBlockDtosQuery(int Amount) : IRequest<IEnumerable<BlockDto>>;

// TODO REDIS - implement cache eventually.

// ReSharper disable once UnusedType.Global
internal sealed class GetLastNBlockDtosQueryHandler(IMapper mapper, IAppDbContext dbContext)
    : IRequestHandler<GetLastNBlockDtosQuery, IEnumerable<BlockDto>>
{
    public async Task<IEnumerable<BlockDto>> Handle(GetLastNBlockDtosQuery request, CancellationToken ct)
    {
        var blocks = await dbContext.Set<Block>()
            .AsNoTracking()
            .OrderBy(x => x.Index).Reverse()
            .Take(request.Amount)
            .ProjectTo<BlockDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return blocks;
    }
}