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
                    break;

                case EntityState.Modified:
                    // Proteger CreatedAt: si llegó como MinValue (→ -infinity en Postgres) corregirlo
                    if (entry.Entity.CreatedAt == DateTime.MinValue)
                        entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedBy = "system";
                    entry.Entity.UpdatedAt = now;
                    break;
            }
        }
    }
}