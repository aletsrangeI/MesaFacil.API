using Domain.Entities;

namespace Interface.Persistence;

public interface ICorteCajaRepository
{
    #region Metodos sincronos
    bool Insert(CorteCaja entity);
    bool Update(CorteCaja entity);
    bool Delete(int id);
    CorteCaja Get(int id);
    IEnumerable<CorteCaja> GetAll();
    IEnumerable<CorteCaja> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(CorteCaja entity);
    Task<bool> UpdateAsync(CorteCaja entity);
    Task<bool> DeleteAsync(int id);
    Task<CorteCaja> GetAsync(int id);
    Task<IEnumerable<CorteCaja>> GetAllAsync();
    Task<IEnumerable<CorteCaja>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}