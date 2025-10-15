using Domain.Entities;

namespace Interface.Persistence;

public interface IDetalleCuentaRepository
{
    #region Metodos sincronos
    bool Insert(DetalleCuenta entity);
    bool Update(DetalleCuenta entity);
    bool Delete(int id);
    DetalleCuenta Get(int id);
    IEnumerable<DetalleCuenta> GetAll();
    IEnumerable<DetalleCuenta> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(DetalleCuenta entity);
    Task<bool> UpdateAsync(DetalleCuenta entity);
    Task<bool> DeleteAsync(int id);
    Task<DetalleCuenta> GetAsync(int id);
    Task<IEnumerable<DetalleCuenta>> GetAllAsync();
    Task<IEnumerable<DetalleCuenta>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}