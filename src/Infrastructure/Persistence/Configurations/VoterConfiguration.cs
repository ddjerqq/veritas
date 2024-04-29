using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal class VoterConfiguration : IEntityTypeConfiguration<Voter>
{
    public void Configure(EntityTypeBuilder<Voter> builder)
    {
        builder.HasKey(x => x.Address);
        builder.Property(x => x.Address)
            .HasMaxLength(44)
            .HasColumnName("addr");

        builder.HasIndex(x => x.PublicKey).IsUnique();
        builder.Property(x => x.PublicKey)
            .HasMaxLength(182)
            .HasColumnName("pkey");

        builder.HasMany(x => x.Votes)
            .WithOne(x => x.Voter)
            .HasForeignKey(x => x.VoterAddress);

        builder.Ignore("Dsa");
        builder.Ignore(x => x.PrivateKey);
    }
}