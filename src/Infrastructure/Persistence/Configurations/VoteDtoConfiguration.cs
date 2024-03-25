using System.ComponentModel;
using Application.Dtos;
using Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

[EditorBrowsable(EditorBrowsableState.Never)]
internal class VoteDtoConfiguration : IEntityTypeConfiguration<VoteDto>
{
    public void Configure(EntityTypeBuilder<VoteDto> builder)
    {
        builder.HasKey(x => x.Hash);

        builder.HasIndex(x => x.VoterAddress).IsUnique();
        builder.Property(x => x.VoterAddress)
            .HasColumnName("addr");

        builder.HasIndex(x => x.PartyId)
            .IsDescending(false);
    }
}