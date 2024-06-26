using Application.Common.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Blockchain.Queries;

public sealed record GetLastBlockQuery : IRequest<Block>;

// ReSharper disable once UnusedType.Global
internal sealed class GetLastBlockQueryHandler(IAppDbContext dbContext) : IRequestHandler<GetLastBlockQuery, Block>
{
    public async Task<Block> Handle(GetLastBlockQuery request, CancellationToken ct)
    {
        return await dbContext.Set<Block>()
            .AsNoTracking()
            .Include(b => b.Votes)
            .ThenInclude(v => v.Voter)
            .OrderBy(x => x.Index)
            .LastAsync(ct);
    }
}