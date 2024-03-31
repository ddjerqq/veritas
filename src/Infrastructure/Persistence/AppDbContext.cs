using System.ComponentModel.DataAnnotations;
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
            Blocks.Add(genesis);
            SaveChanges(true);
        }
    }

    public void EnsureBlockchainIsConsistent()
    {
        ValidateBlocks();
        ValidateVotes();
    }

    private void ValidateBlocks()
    {
        var blocks = Blocks
            .Include(b => b.Votes)
            .ThenInclude(v => v.Voter)
            .ToList();

        for (var i = 0; i < blocks.Count; i++)
        {
            var block = blocks[i];

            if (!block.IsHashValid)
                throw new ValidationException($"Block hash is not valid. Block Index: {block.Index}");

            var previous = i == 0 ? null : blocks[i - 1];
            if (previous is not null && block.PreviousHash != previous.Hash)
                throw new ValidationException($"Block hash does not equal the previous hash. expected: {previous.Hash} but was: {block.Hash}");
        }
    }

    private void ValidateVotes()
    {
        var votes = Votes
            .Include(v => v.Voter)
            .ToList();

        foreach (var vote in votes)
        {
            if (!vote.IsHashValid)
                throw new ValidationException($"Vote hash is not valid. Hash: {vote.Hash}");

            if (!vote.IsSignatureValid)
                throw new ValidationException($"Vote signature is not valid. Hash: {vote.Hash} Signature: {vote.Signature}");
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