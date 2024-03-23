using Api.Common;
using Api.Data.Dto;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<BlockDto> Blocks => Set<BlockDto>();

    public DbSet<VoteDto> Votes => Set<VoteDto>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(builder);
        SnakeCaseRename(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableDetailedErrors();
        optionsBuilder.EnableSensitiveDataLogging();
        base.OnConfiguring(optionsBuilder);
    }

    private static void SnakeCaseRename(ModelBuilder builder)
    {
        builder.UsePropertyAccessMode(PropertyAccessMode.PreferFieldDuringConstruction);

        foreach (var entity in builder.Model.GetEntityTypes())
        {
            var entityTableName = entity.GetTableName()!
                // identity tables fix
                .Replace("AspNet", string.Empty)
                .TrimEnd('s')
                .ToSnakeCase();

            entity.SetTableName(entityTableName);

            foreach (var property in entity.GetProperties())
                property.SetColumnName(property.GetColumnName().ToSnakeCase());

            foreach (var key in entity.GetKeys())
                key.SetName(key.GetName()?.ToSnakeCase());

            foreach (var key in entity.GetForeignKeys())
                key.SetConstraintName(key.GetConstraintName()?.ToSnakeCase());

            foreach (var index in entity.GetIndexes())
                index.SetDatabaseName(index.GetDatabaseName()?.ToSnakeCase());
        }
    }
}