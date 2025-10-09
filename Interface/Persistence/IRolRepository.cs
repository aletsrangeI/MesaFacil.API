using Domain.Entities;

namespace Interface.Persistence;

public interface IRolRepository
{
    #region Metodos sincronos
    bool Insert(Rol entity);
    bool Update(Rol entity);
    bool Delete(int id);
    Rol Get(int id);
    IEnumerable<Rol> GetAll();
    IEnumerable<Rol> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(Rol entity);
    Task<bool> UpdateAsync(Rol entity);
    Task<bool> DeleteAsync(int id);
    Task<Rol> GetAsync(int id);
    Task<IEnumerable<Rol>> GetAllAsync();
    Task<IEnumerable<Rol>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}