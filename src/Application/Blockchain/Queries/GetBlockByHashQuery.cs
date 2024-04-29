using Application.Common.Abstractions;
using Application.Dto;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Blockchain.Queries;

public sealed record GetBlockByHashQuery(string Hash) : IRequest<BlockDto?>;

// TODO REDIS - implement cache eventually.

// ReSharper disable once UnusedType.Global
internal sealed class GetBlockByHashQueryHandler(IMapper mapper, IAppDbContext dbContext) : IRequestHandler<GetBlockByHashQuery, BlockDto?>
{
    public async Task<BlockDto?> Handle(GetBlockByHashQuery request, CancellationToken ct)
    {
        var block = await dbContext.Set<Block>()
            .AsNoTracking()
            .Where(b => b.Hash == request.Hash)
            .ProjectTo<BlockDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

        return block;
    }
}