using Application.Abstractions;
using Application.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Aggregates;
using Domain.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services;

public sealed class InMemoryBlockCache : IBlockCache
{
    public List<Block> Blocks { get; init; } = [];

    private Block? _current;

    public Block GetCurrent()
    {
        _current ??= Blocks.Last();

        if (_current.Index == 0)
        {
            _current = _current.Next();
            Blocks.Add(_current);
        }

        return _current;
    }

    public Block? GetByIndex(long index) => Blocks.FirstOrDefault(x => x.Index == index);

    public Block? GetByHash(string hash) => Blocks.FirstOrDefault(x => x.Hash.ToHexString() == hash);

    public void Add(Block block)
    {
        if (!block.IsHashValid)
            throw new InvalidOperationException("the blocks hash is not valid");

        Blocks.Add(block);
        _current = block;
    }

    public Block MineAndRotate()
    {
        var block = GetCurrent();
        var minedBlock = block.Mine();

        Blocks.Remove(_current!);
        Blocks.Add(minedBlock);

        var next = minedBlock.Next();
        Blocks.Add(next);

        _current = next;

        return minedBlock;
    }
}

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryBlockCache(this IServiceCollection services)
    {
        services.AddSingleton<IBlockCache, InMemoryBlockCache>(sp =>
        {
            using var scope = sp.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

            var blocks = dbContext
                .Set<BlockDto>()
                .OrderBy(b => b.Index)
                .ProjectTo<Block>(mapper.ConfigurationProvider)
                .ToList();

            return new InMemoryBlockCache
            {
                Blocks = blocks,
            };
        });

        return services;
    }
}