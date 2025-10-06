using Domain.Entities;

namespace Interface.Persistence;

public interface IAreaRepository
{
    #region Metodos sincronos
    bool Insert(Area entity);
    bool Update(Area entity);
    bool Delete(int id);
    Area Get(int id);
    IEnumerable<Area> GetAll();
    IEnumerable<Area> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(Area entity);
    Task<bool> UpdateAsync(Area entity);
    Task<bool> DeleteAsync(int id);
    Task<Area> GetAsync(int id);
    Task<IEnumerable<Area>> GetAllAsync();
    Task<IEnumerable<Area>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}