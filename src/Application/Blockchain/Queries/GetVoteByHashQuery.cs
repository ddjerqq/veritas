using Application.Common.Abstractions;
using Application.Dto;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Blockchain.Queries;

public sealed record GetVoteByHashQuery(string Hash) : IRequest<VoteDto?>;

// TODO REDIS - implement cache eventually.

// ReSharper disable once UnusedType.Global
internal sealed class GetVoteByHashQueryHandler(IMapper mapper, IAppDbContext dbContext) : IRequestHandler<GetVoteByHashQuery, VoteDto?>
{
    public async Task<VoteDto?> Handle(GetVoteByHashQuery request, CancellationToken ct)
    {
        return await dbContext.Set<Vote>()
            .AsNoTracking()
            .Where(v => v.Hash == request.Hash)
            .ProjectTo<VoteDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }
}