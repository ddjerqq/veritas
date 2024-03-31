using Application.Common.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Blockchain.Queries;

public sealed record GetAllBlocksQuery : IRequest<IEnumerable<Block>>;

// TODO REDIS - implement cache eventually.

// ReSharper disable once UnusedType.Global
public sealed class AllBlocksQueryHandler(IAppDbContext dbContext) : IRequestHandler<GetAllBlocksQuery, IEnumerable<Block>>
{
    public async Task<IEnumerable<Block>> Handle(GetAllBlocksQuery request, CancellationToken ct)
    {
        return await dbContext.Set<Block>()
            .AsNoTracking()
            .Include(b => b.Votes)
            .ThenInclude(v => v.Voter)
            .ToListAsync(ct);
    }
}