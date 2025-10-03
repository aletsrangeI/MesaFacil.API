namespace Interface.Persistence;

public interface IUnitOfWork : IDisposable
{
    ICatalogRepository Catalogs { get; }
}