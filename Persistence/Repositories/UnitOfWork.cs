using Interface.Persistence;
using Persistence.Context;

namespace Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    public ICatalogRepository Catalogs { get; }

    private readonly ApplicationDbContext _context;

    public ICatalogItemRepository CatalogItems { get; }

    public IAreaRepository Areas { get; }

    public ICategoriaMenuRepository CategoriaMenus { get; }
    public UnitOfWork(ApplicationDbContext context,
        ICatalogRepository catalogRepository,
        ICatalogItemRepository catalogItems,
        IAreaRepository areaRepository, 
        ICategoriaMenuRepository categoriaMenuRepository)
    {
        CategoriaMenus = categoriaMenuRepository;
        Areas = areaRepository;
        CatalogItems = catalogItems;
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
