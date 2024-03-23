using Api.Common;
using Api.Data;
using Api.Data.Dto;
using Api.Data.Models;
using Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class Blockchain : IBlockChain
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<Blockchain> _logger;
    private readonly List<Block> _blocks;

    private readonly SemaphoreSlim _lock = new(1, 1);

    public Block CurrentBlock { get; }

    public IReadOnlyCollection<Block> Blocks => _blocks.AsReadOnly();

    public Blockchain(AppDbContext dbContext, ILogger<Blockchain> logger)
    {
        _dbContext = dbContext;
        _logger = logger;

        _blocks = _dbContext.Blocks
            .Include(dto => dto.Votes)
            .AsEnumerable()
            .Select(dto => (Block)dto)
            .ToList();

        CurrentBlock = _blocks.LastOrDefault() ?? Block.Genesis();
    }

    public async Task<bool> TryAddVoteAsync(Vote vote, CancellationToken ct = default)
    {
        // check for voter anywhere
        if (_blocks.Any(b => b.Votes.Any(v => v.Voter.Address == vote.Voter.Address)))
            return false;

        try
        {
            // so we dont have race conditions, because this is going to be a singleton class.
            await _lock.WaitAsync(ct);

            var currentBlock = CurrentBlock;
            if (currentBlock.Votes.Count == Block.VoteLimit)
            {
                // mine
                var minedBlock = currentBlock.Mine();
                _blocks[_blocks.Count] = minedBlock;
                _dbContext.Blocks.Add((BlockDto)minedBlock);
                await _dbContext.SaveChangesAsync(ct);
            }
            else
            {
                return currentBlock.TryAddVote(vote);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error while adding vote to the chain");
            return false;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<Block?> GetByIndexAsync(long blockIndex, CancellationToken ct = default)
    {
        var found = _blocks.FirstOrDefault(b => b.Index == blockIndex);
        found ??= (Block?)(await _dbContext.Blocks.FirstOrDefaultAsync(b => b.Index == blockIndex, ct))!;
        return found;
    }

    public async Task<Block?> GetByHashAsync(string blockHash, CancellationToken ct = default)
    {
        var found = _blocks.FirstOrDefault(b => b.Hash.ToHexString() == blockHash);
        found ??= (Block?)(await _dbContext.Blocks.FirstOrDefaultAsync(b => b.Hash == blockHash, ct))!;
        return found;
    }
}