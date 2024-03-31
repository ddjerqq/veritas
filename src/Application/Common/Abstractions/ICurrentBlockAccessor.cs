﻿using Domain.Entities;

namespace Application.Common.Abstractions;

public interface ICurrentBlockAccessor
{
    protected Block? Current { get; set; }

    public Task<Block> GetCurrentBlockAsync(CancellationToken ct = default);

    public void SetCurrent(Block block) => Current = block;
}