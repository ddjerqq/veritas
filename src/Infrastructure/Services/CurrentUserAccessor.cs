using Application.Abstractions;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Infrastructure.Services;

public sealed class CurrentUserAccessor(IHttpContextAccessor httpContextAccessor, IAppDbContext dbContext) : ICurrentUserAccessor
{
    public async Task<Voter?> GetCurrentUserAsync(CancellationToken ct = default)
    {
        var stringId = httpContextAccessor
            .HttpContext?
            .User
            .Claims
            .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid)?
            .Value;

        Guid? currentUserId = Guid.TryParse(stringId, out var id)
            ? id
            : null;

        if (currentUserId is null)
            return null;

        // return await dbContext.Set<User>().FirstOrDefaultAsync(u => u.Id == id, ct);
        throw new NotImplementedException();
    }
}