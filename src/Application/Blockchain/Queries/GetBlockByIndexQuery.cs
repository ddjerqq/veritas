using Application.Common.Abstractions;
using Application.Dto;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Blockchain.Queries;

public sealed record GetBlockByIndexQuery(long Index) : IRequest<BlockDto?>;

// TODO REDIS - implement cache eventually.

// ReSharper disable once UnusedType.Global
internal sealed class GetBlockByIndexQueryHandler(IMapper mapper, IAppDbContext dbContext) : IRequestHandler<GetBlockByIndexQuery, BlockDto?>
{
    public async Task<BlockDto?> Handle(GetBlockByIndexQuery request, CancellationToken ct)
    {
        var block = await dbContext.Set<Block>()
            .AsNoTracking()
            .Where(b => b.Index == request.Index)
            .ProjectTo<BlockDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

        return block;
    }
}