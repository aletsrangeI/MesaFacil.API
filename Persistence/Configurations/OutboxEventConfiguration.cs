using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class OutboxEventConfiguration : IEntityTypeConfiguration<OutboxEvent>
{
    public void Configure(EntityTypeBuilder<OutboxEvent> e)
    {
        e.ToTable("OutboxEvents");
        e.HasKey(x => x.Id);

        e.Property(x => x.AggregateType).IsRequired().HasMaxLength(50);
        e.Property(x => x.EventType).IsRequired().HasMaxLength(50);
        e.Property(x => x.SyncStatus).IsRequired().HasMaxLength(20).HasDefaultValue(OutboxSyncStatus.Pending);
        e.Property(x => x.PayloadJson).IsRequired();
        e.Property(x => x.RetryCount).HasDefaultValue(0);

        e.HasIndex(x => new { x.SyncStatus, x.CreatedAt });
        e.HasIndex(x => x.AggregateId);
    }
}
