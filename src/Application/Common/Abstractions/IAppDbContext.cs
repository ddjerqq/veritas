using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Abstractions;

public interface IAppDbContext : IDisposable
{
    public DbSet<TEntity> Set<TEntity>() where TEntity : class;

    public Task<int> SaveChangesAsync(CancellationToken ct = default);

    public void AddDomainEvent(IDomainEvent ev, IDateTimeProvider dateTimeProvider)
    {
        var msg = OutboxMessage.FromDomainEvent(ev, dateTimeProvider);
        Set<OutboxMessage>().Add(msg);
    }
}