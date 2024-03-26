using Application.Abstractions;
using Domain.Common;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Infrastructure.Services;

public sealed class CurrentVoterAccessor(IHttpContextAccessor httpContextAccessor) : ICurrentVoterAccessor
{
    public Voter? GetCurrentVoter(CancellationToken ct = default)
    {
        var pubKey = httpContextAccessor
            .HttpContext?
            .User
            .Claims
            .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid)?
            .Value;

        return pubKey is null
            ? null
            : Voter.FromPubKey(pubKey.ToBytesFromHex());
    }
}