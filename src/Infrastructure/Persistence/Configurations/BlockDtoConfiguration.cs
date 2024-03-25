using System.ComponentModel;
using Application.Dtos;
using Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

[EditorBrowsable(EditorBrowsableState.Never)]
internal class BlockDtoConfiguration : IEntityTypeConfiguration<BlockDto>
{
    public void Configure(EntityTypeBuilder<BlockDto> builder)
    {
        builder.HasKey(x => x.Index);
        builder.Property(x => x.Index)
            .HasColumnName("idx");

        builder.Property(x => x.MerkleRoot)
            .HasColumnName("mrkl_root");

        builder.HasIndex(x => x.Hash).IsUnique();

        // TODO after we make conversions, add genesis.
        // builder.HasData(Block.Genesis());
    }
}