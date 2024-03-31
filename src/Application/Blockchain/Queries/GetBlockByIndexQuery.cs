using Application.Common.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Blockchain.Queries;

public sealed record GetBlockByIndexQuery(long Index) : IRequest<Block?>;

// TODO implement REDIS cache eventually.

// ReSharper disable once UnusedType.Global
public sealed class GetBlockByIndexQueryHandler(IAppDbContext dbContext) : IRequestHandler<GetBlockByIndexQuery, Block?>
{
    public async Task<Block?> Handle(GetBlockByIndexQuery request, CancellationToken ct)
    {
        return await dbContext.Set<Block>()
            .AsNoTracking()
            .Include(b => b.Votes)
            .ThenInclude(v => v.Voter)
            .Where(b => b.Index == request.Index)
            .FirstOrDefaultAsync(ct);
    }
}