using Microsoft.EntityFrameworkCore;

namespace Application.Common;

public static class DbSetExtensions
{
    public static bool TryUpdate<TEntity>(this DbSet<TEntity> set, TEntity entity) where TEntity : class
    {
        try
        {
            set.Update(entity);
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
}