using Domain.Entities;

namespace Interface.Persistence;

public interface ICuentaRepository
{
    #region Metodos sincronos
    bool Insert(Cuenta entity);
    bool Update(Cuenta entity);
    bool Delete(int id);
    Cuenta Get(int id);
    IEnumerable<Cuenta> GetAll();
    IEnumerable<Cuenta> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(Cuenta entity);
    Task<bool> UpdateAsync(Cuenta entity);
    Task<bool> DeleteAsync(int id);
    Task<Cuenta> GetAsync(int id);
    Task<IEnumerable<Cuenta>> GetAllAsync();
    Task<IEnumerable<Cuenta>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}