using Application.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.Property(x => x.Type).HasMaxLength(256);
        builder.Property(x => x.Content).HasMaxLength(2048);
        builder.Property(x => x.Error).HasMaxLength(4096);
    }
}