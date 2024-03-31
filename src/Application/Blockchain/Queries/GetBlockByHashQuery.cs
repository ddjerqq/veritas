using Application.Common.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Blockchain.Queries;

public sealed record GetBlockByHashQuery(string Hash) : IRequest<Block?>;

// TODO implement REDIS cache eventually.

// ReSharper disable once UnusedType.Global
public sealed class GetBlockByHashQueryHandler(IAppDbContext dbContext) : IRequestHandler<GetBlockByHashQuery, Block?>
{
    public async Task<Block?> Handle(GetBlockByHashQuery request, CancellationToken ct)
    {
        return await dbContext.Set<Block>()
            .AsNoTracking()
            .Include(b => b.Votes)
            .ThenInclude(v => v.Voter)
            .Where(b => b.Hash == request.Hash)
            .FirstOrDefaultAsync(ct);
    }
}