using Application.Common.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Blockchain.Queries;

public sealed record AllBlocksQuery : IRequest<IEnumerable<Block>>;

public sealed class AllBlocksQueryHandler(IAppDbContext dbContext) : IRequestHandler<AllBlocksQuery, IEnumerable<Block>>
{
    // TODO implement REDIS cache eventually.
    public async Task<IEnumerable<Block>> Handle(AllBlocksQuery request, CancellationToken ct)
    {
        return await dbContext.Set<Block>()
            .Include(b => b.Votes)
            .ThenInclude(v => v.Voter)
            .ToListAsync(ct);
    }
}