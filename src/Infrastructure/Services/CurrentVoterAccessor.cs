using Application.Abstractions;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public sealed class CurrentVoterAccessor(IHttpContextAccessor httpContextAccessor) : ICurrentVoterAccessor
{
    public Voter? GetCurrentVoter(CancellationToken ct = default)
    {
        var ctx = httpContextAccessor.HttpContext;
        if (ctx is null) return null;

        if (ctx.Items.TryGetValue(nameof(Voter), out var value) && value is Voter voter)
            return voter;

        return null;
    }
}