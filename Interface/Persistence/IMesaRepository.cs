using Domain.Entities;

namespace Interface.Persistence;

public interface IMesaRepository
{
    #region Metodos sincronos
    bool Insert(Mesa entity);
    bool Update(Mesa entity);
    bool Delete(int id);
    Mesa Get(int id);
    IEnumerable<Mesa> GetAll();
    IEnumerable<Mesa> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(Mesa entity);
    Task<bool> UpdateAsync(Mesa entity);
    Task<bool> DeleteAsync(int id);
    Task<Mesa> GetAsync(int id);
    Task<IEnumerable<Mesa>> GetAllAsync();
    Task<IEnumerable<Mesa>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}