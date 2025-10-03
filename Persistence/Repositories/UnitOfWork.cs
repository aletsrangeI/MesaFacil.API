using Interface.Persistence;
using Persistence.Context;

namespace Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    public ICatalogRepository Catalogs { get; }

    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context,
        ICatalogRepository catalogRepository)
    {
        _context = context;
        Catalogs = catalogRepository;
    }

    public async Task<int> Save(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}