using System.ComponentModel;
using Api.Data;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Api.Services;

[EditorBrowsable(EditorBrowsableState.Never)]
internal class BlockchainTest
{
    private AppDbContext _dbContext = default!;

    [SetUp]
    public void SetUp()
    {
        var dbPath = Environment.GetEnvironmentVariable("DB__PATH") ?? "C:/work/mieci/src/app.db";

        _dbContext = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite($"Data Path={dbPath}")
            .Options);
    }

    [Test]
    [NonParallelizable]
    public void TestLoad()
    {

    }
}