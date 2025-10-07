using Domain.Entities;

namespace Interface.Persistence;

public interface ICredencialRepository
{
    #region Metodos sincronos
    bool Insert(Credencial entity);
    bool Update(Credencial entity);
    bool Delete(int id);
    Credencial Get(int id);
    IEnumerable<Credencial> GetAll();
    IEnumerable<Credencial> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(Credencial entity);
    Task<bool> UpdateAsync(Credencial entity);
    Task<bool> DeleteAsync(int id);
    Task<Credencial> GetAsync(int id);
    Task<IEnumerable<Credencial>> GetAllAsync();
    Task<IEnumerable<Credencial>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}