using Application.Common.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Blockchain.Queries;

public sealed record VoterInfo
{
    public string Address { get; set; } = default!;

    public string PublicKey { get; set; } = default!;

    public IEnumerable<Vote> Votes { get; set; } = [];
}

public sealed record GetVoterInfoQuery(string Address) : IRequest<VoterInfo?>;

// ReSharper disable once UnusedType.Global
public sealed class GetVoterInfoQueryHandler(IAppDbContext dbContext) : IRequestHandler<GetVoterInfoQuery, VoterInfo?>
{
    public async Task<VoterInfo?> Handle(GetVoterInfoQuery request, CancellationToken ct)
    {
        var voter = await dbContext.Set<Voter>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Address == request.Address, ct);

        if (voter is null) return null;

        var votes = await dbContext.Set<Vote>()
            .Where(x => x.Voter.Address == request.Address)
            .ToListAsync(ct);

        return new VoterInfo
        {
            Address = voter.Address,
            PublicKey = voter.PublicKey,
            Votes = votes,
        };
    }
}