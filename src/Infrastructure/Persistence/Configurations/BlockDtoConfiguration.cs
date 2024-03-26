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

        var genesis = new BlockDto
        {
            Index = 0,
            Nonce = 14261917,
            Hash = "000000983e2e5dae2c718cbd3d495695f5dc8c489375bcc3ce65806d9536ea59",
            MerkleRoot = new string('0', 64),
            PreviousHash = new string('0', 64),
            Votes = [],
        };
        builder.HasData(genesis);
    }
}