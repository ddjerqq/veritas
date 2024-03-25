using Domain.ValueObjects;

namespace Application.Abstractions;

public interface ICurrentUserAccessor
{
    public Task<Voter?> GetCurrentUserAsync(CancellationToken ct = default);
}