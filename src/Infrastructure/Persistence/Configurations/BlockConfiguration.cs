using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal class BlockConfiguration : IEntityTypeConfiguration<Block>
{
    public void Configure(EntityTypeBuilder<Block> builder)
    {
        builder.HasKey(x => x.Index);

        builder.Property(x => x.Index)
            .HasColumnName("idx")
            .ValueGeneratedNever();

        builder.HasIndex(x => x.Hash).IsUnique();
        builder.Property(x => x.Hash)
            .HasMaxLength(64);

        builder.Property(x => x.MerkleRoot)
            .HasMaxLength(64)
            .HasColumnName("mrkl_root");

        builder.Property(x => x.PreviousHash)
            .HasMaxLength(64);

        builder.HasMany(x => x.Votes)
            .WithOne(vote => vote.Block)
            .HasForeignKey(vote => vote.BlockIndex);
    }
}