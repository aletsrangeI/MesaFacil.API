using Domain.Entities;

namespace Interface.Persistence;

public interface ICategoriaMenuRepository
{
    #region Metodos sincronos
    bool Insert(CategoriaMenu entity);
    bool Update(CategoriaMenu entity);
    bool Delete(int id);
    CategoriaMenu Get(int id);
    IEnumerable<CategoriaMenu> GetAll();
    IEnumerable<CategoriaMenu> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(CategoriaMenu entity);
    Task<bool> UpdateAsync(CategoriaMenu entity);
    Task<bool> DeleteAsync(int id);
    Task<CategoriaMenu> GetAsync(int id);
    Task<IEnumerable<CategoriaMenu>> GetAllAsync();
    Task<IEnumerable<CategoriaMenu>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}