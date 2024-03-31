using Microsoft.EntityFrameworkCore;
using Test.Infrastructure.Data;
using Test.Infrastructure.Data.Models;

namespace Test.Infrastructure;

public class TestCrud
{
    private readonly TestDbContext _dbContext = new();

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _dbContext.Dispose();
    }

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        await _dbContext.InitDbAsync();
    }

    [Test]
    [NonParallelizable]
    public async Task TestCreate()
    {
        var ts = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var voter = new Voter
        {
            Address = "address",
            PublicKey = "public_key",
        };

        var vote = new Vote
        {
            Nonce = 0,
            PartyId = 0,
            Voter = voter,
            Timestamp = ts,
        };

        var block = new Block
        {
            Index = ts / 2,
            Nonce = 0,
            PreviousHash = new string('0', 64),
        };

        vote.Block = block;
        vote.BlockIndex = block.Index;
        block.AddVote(vote);

        _dbContext.Blocks.Add(block);
        await _dbContext.SaveChangesAsync();

        block = await _dbContext.Blocks
            .Include(b => b.Votes)
            .ThenInclude(v => v.Voter)
            .FirstOrDefaultAsync(b => b.Index == block.Index);

        Assert.That(block, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(block.Votes, Has.Count.EqualTo(1));
            Assert.That(block.Votes.First().Hash, Is.EqualTo(vote.Hash));
            Assert.That(block.Votes.First().Signature, Is.EqualTo(vote.Signature));
            Assert.That(block.Votes.First().Timestamp, Is.EqualTo(vote.Timestamp));
        });
    }

    [Test]
    [NonParallelizable]
    public async Task TestRead()
    {
        var blocks = await _dbContext.Blocks
            .Include(b => b.Votes)
            .ThenInclude(v => v.Voter)
            .ToListAsync();

        Assert.Multiple(() =>
        {
            Assert.That(blocks[0].Votes.First().Hash, Has.Length.EqualTo(64));
            Assert.That(blocks[0].Votes.First().Signature, Has.Length.EqualTo(128));
            Assert.That(blocks[0].Votes.First().Timestamp, Is.EqualTo(0));
        });
    }

    [Test]
    [NonParallelizable]
    public async Task TestUpdate()
    {
        // only case for update will be after user votes, the vote's blockIndex will change.
        var vote = await _dbContext.Votes.FirstAsync();
        var block = await _dbContext.Blocks.FirstAsync();

        vote.BlockIndex = block.Index;

        await _dbContext.SaveChangesAsync();

        var votes = await _dbContext.Votes
            .Include(v => v.Voter)
            .ToListAsync();

        Assert.That(votes, Has.Count.EqualTo(1));
        Assert.That(votes[0].BlockIndex, Is.EqualTo(block.Index));
    }

    [Test]
    [NonParallelizable]
    public async Task TestDeleteIsForbidden()
    {
        var block = await _dbContext.Blocks.FirstAsync();
        _dbContext.Blocks.Remove(block);

        var ex = Assert.Throws<DbUpdateException>(() => _dbContext.SaveChanges());
        Assert.That(ex.InnerException, Is.InstanceOf<Exception>());
    }
}