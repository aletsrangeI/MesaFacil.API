namespace Interface.Persistence;

public interface IUnitOfWork : IDisposable
{
    ICatalogRepository Catalogs { get; }
    ICatalogItemRepository CatalogItems { get; }
    IAreaRepository Areas { get; }
    ICategoriaMenuRepository CategoriaMenus { get; }
}
