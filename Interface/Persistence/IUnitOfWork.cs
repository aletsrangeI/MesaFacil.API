namespace Interface.Persistence;

public interface IUnitOfWork : IDisposable
{
    ICatalogRepository Catalogs { get; }
    ICatalogItemRepository CatalogItems { get; }
    IAreaRepository Areas { get; }
    ICategoriaMenuRepository CategoriaMenus { get; }
    IClienteRepository Clientes { get; }
    ICorteCajaRepository CorteCajas { get; }
    ICredencialRepository Credenciales { get; }
}
