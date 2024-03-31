using Application.Common;
using Application.Common.Abstractions;
using Domain.Common;
using Domain.Entities;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Persistence.ValueConverters;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options,
    ConvertDomainEventsToOutboxMessagesInterceptor convertDomainEventsToOutboxMessagesInterceptor)
    : DbContext(options), IAppDbContext
{
    public DbSet<Block> Blocks => Set<Block>();

    public DbSet<Vote> Votes => Set<Vote>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(builder);
        SnakeCaseRename(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(convertDomainEventsToOutboxMessagesInterceptor);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder
            .Properties<DateTime>()
            .HaveConversion<DateTimeUtcValueConverter>();

        base.ConfigureConventions(builder);
    }

    public void SeedGenesisBlock()
    {
        if (!Blocks.AsNoTracking().Any())
        {
            var genesis = Block.GenesisBlock();
            var next = genesis.NextBlock();

            Blocks.Add(genesis);
            Blocks.Add(next);

            SaveChanges(true);
        }
    }

    private static void SnakeCaseRename(ModelBuilder builder)
    {
        foreach (var entity in builder.Model.GetEntityTypes())
        {
            var entityTableName = entity.GetTableName()!
                .Replace("AspNet", string.Empty)
                .TrimEnd('s')
                .ToSnakeCase();

            entity.SetTableName(entityTableName);

            foreach (var property in entity.GetProperties())
                property.SetColumnName(property.GetColumnName().ToSnakeCase());

            foreach (var key in entity.GetKeys())
                key.SetName(key.GetName()!.ToSnakeCase());

            foreach (var key in entity.GetForeignKeys())
                key.SetConstraintName(key.GetConstraintName()!.ToSnakeCase());

            foreach (var index in entity.GetIndexes())
                index.SetDatabaseName(index.GetDatabaseName()!.ToSnakeCase());
        }
    }
}