using Domain.Entities;

namespace Interface.Persistence;

public interface ISucursalRepository
{
    #region Metodos sincronos
    bool Insert(Sucursal entity);
    bool Update(Sucursal entity);
    bool Delete(int id);
    Sucursal Get(int id);
    IEnumerable<Sucursal> GetAll();
    IEnumerable<Sucursal> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(Sucursal entity);
    Task<bool> UpdateAsync(Sucursal entity);
    Task<bool> DeleteAsync(int id);
    Task<Sucursal> GetAsync(int id);
    Task<IEnumerable<Sucursal>> GetAllAsync();
    Task<IEnumerable<Sucursal>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}