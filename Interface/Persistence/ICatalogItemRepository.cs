using Domain.Entities;

namespace Interface.Persistence;

public interface ICatalogItemRepository
{
    #region Metodos sincronos
    bool Insert(CatalogItem entity);
    bool Update(CatalogItem entity);
    bool Delete(int id);
    CatalogItem Get(int id);
    IEnumerable<CatalogItem> GetAll();
    IEnumerable<CatalogItem> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(CatalogItem entity);
    Task<bool> UpdateAsync(CatalogItem entity);
    Task<bool> DeleteAsync(int id);
    Task<CatalogItem> GetAsync(int id);
    Task<IEnumerable<CatalogItem>> GetAllAsync();
    Task<IEnumerable<CatalogItem>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}