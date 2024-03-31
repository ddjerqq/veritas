using Microsoft.EntityFrameworkCore;
using Test.Infrastructure.Data.Models;

namespace Test.Infrastructure.Data;

public class TestDbContext : DbContext
{
    public DbSet<Block> Blocks => Set<Block>();

    public DbSet<Vote> Votes => Set<Vote>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // var dbPath = Path.Combine(_workDir.FullName, "test.db");
        var dbPath = "C:/work/mieci/test/test.Infrastructure/test.db";
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }

    public async Task InitDbAsync()
    {
        await Database.MigrateAsync();
    }
}