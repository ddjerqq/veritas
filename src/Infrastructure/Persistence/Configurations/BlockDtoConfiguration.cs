using Application.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

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

        builder.HasMany(x => x.Votes)
            .WithOne(vote => vote.Block)
            .HasForeignKey(vote => vote.BlockIndex);
    }
}