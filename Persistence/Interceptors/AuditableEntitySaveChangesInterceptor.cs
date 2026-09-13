using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Persistence.Interceptors;

public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void UpdateEntities(DbContext? context)
    {
        if (context is null) return;

        var now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

        foreach (var entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy ??= "system";
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedBy ??= "system";
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.IsActive = true;
                    break;

                case EntityState.Modified:
                    // Proteger CreatedAt/CreatedBy si vienen vacíos de un DTO mapping
                    if (entry.Entity.CreatedAt == DateTime.MinValue)
                        entry.Entity.CreatedAt = now;
                    if (string.IsNullOrEmpty(entry.Entity.CreatedBy))
                        entry.Entity.CreatedBy = "system";

                    entry.Entity.UpdatedBy = "system";
                    entry.Entity.UpdatedAt = now;
                    break;
            }
        }

        // Spec 019: mismas reglas de auditoría para las entidades operativas con Id Guid/UUIDv7
        foreach (var entry in context.ChangeTracker.Entries<BaseAuditableGuidEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy ??= "system";
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedBy ??= "system";
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.IsActive = true;
                    break;

                case EntityState.Modified:
                    if (entry.Entity.CreatedAt == DateTime.MinValue)
                        entry.Entity.CreatedAt = now;
                    if (string.IsNullOrEmpty(entry.Entity.CreatedBy))
                        entry.Entity.CreatedBy = "system";

                    entry.Entity.UpdatedBy = "system";
                    entry.Entity.UpdatedAt = now;
                    break;
            }
        }
    }
}