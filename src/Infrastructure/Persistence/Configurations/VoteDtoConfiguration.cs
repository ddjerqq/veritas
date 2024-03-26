using Application.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal class VoteDtoConfiguration : IEntityTypeConfiguration<VoteDto>
{
    public void Configure(EntityTypeBuilder<VoteDto> builder)
    {
        builder.HasKey(x => x.Hash);

        builder.HasIndex(x => x.VoterAddress).IsUnique();

        builder.HasIndex(x => x.PartyId)
            .IsDescending(false);
    }
}