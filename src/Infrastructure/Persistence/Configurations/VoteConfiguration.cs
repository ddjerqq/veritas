using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal class VoteConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        builder.HasKey(x => x.Hash);

        builder.Property(x => x.Hash)
            .HasMaxLength(64)
            .ValueGeneratedNever();

        builder.Property(x => x.VoterAddress)
            .HasMaxLength(44)
            .HasColumnName("addr");

        builder.HasOne(x => x.Voter)
            .WithMany()
            .HasForeignKey(x => x.VoterAddress);

        builder.HasIndex(x => x.VoterAddress)
            .IsDescending(false)
            .IsUnique(false);

        builder.HasIndex(x => x.PartyId)
            .IsDescending(false);

        builder.HasOne(x => x.Block)
            .WithMany(b => b.Votes)
            .HasForeignKey(x => x.BlockIndex)
            .IsRequired(false);

        builder.Ignore(x => x.IsHashValid);
        builder.Ignore(x => x.IsSignatureValid);
    }
}