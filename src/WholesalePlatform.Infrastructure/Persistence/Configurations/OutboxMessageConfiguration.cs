using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WholesalePlatform.Infrastructure.Outbox;

namespace WholesalePlatform.Infrastructure.Persistence.Configurations;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");

        builder.HasKey(message => message.Id);

        builder.Property(message => message.Type)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(message => message.Payload)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(message => message.Error)
            .HasMaxLength(2000);

        builder.Property(message => message.LockedBy)
            .HasMaxLength(128);

        builder.Property(message => message.LockedAt);

        builder.Property(message => message.LockExpiresAt);

        builder.Property(message => message.Version)
            .IsConcurrencyToken()
            .IsRequired();

        builder.HasIndex(message => new { message.ProcessedAt, message.RetryCount, message.OccurredAt });
        builder.HasIndex(message => message.LockExpiresAt);
        builder.HasQueryFilter(message => !message.IsDeleted);
    }
}
