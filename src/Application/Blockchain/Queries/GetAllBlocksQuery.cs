using Application.Common.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Blockchain.Queries;

public sealed record GetAllBlocksQuery(int Page) : IRequest<IEnumerable<Block>>
{
    public const int PerPage = 25;
}

// TODO REDIS - implement cache eventually.

// ReSharper disable once UnusedType.Global
internal sealed class AllBlocksQueryHandler(IAppDbContext dbContext) : IRequestHandler<GetAllBlocksQuery, IEnumerable<Block>>
{
    public async Task<IEnumerable<Block>> Handle(GetAllBlocksQuery request, CancellationToken ct)
    {
        return await dbContext.Set<Block>()
            .AsNoTracking()
            .Include(b => b.Votes)
            .ThenInclude(v => v.Voter)
            .OrderBy(x => x.Index)
            .Skip(request.Page * GetAllBlocksQuery.PerPage)
            .Take(GetAllBlocksQuery.PerPage)
            .ToListAsync(ct);
    }
}