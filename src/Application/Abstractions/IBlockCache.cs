using Domain.Aggregates;

namespace Application.Abstractions;

public interface IBlockCache
{
    public Block GetCurrent();

    public Block? GetByIndex(long index);

    public Block? GetByHash(string hash);

    public void Add(Block block);

    /// <summary>
    /// This method will mine the current block and return the mined block.
    /// It will set its predecessor to the current block.
    /// </summary>
    /// <note>
    /// It is the caller's responsibility to add the mined block to the database
    /// </note>
    public Block MineAndRotate();
}