using Domain.Entities;

namespace Interface.Persistence;

public interface IMovimientoCajaRepository
{
    #region Metodos sincronos
    bool Insert(MovimientoCaja entity);
    bool Update(MovimientoCaja entity);
    bool Delete(int id);
    MovimientoCaja Get(int id);
    IEnumerable<MovimientoCaja> GetAll();
    IEnumerable<MovimientoCaja> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(MovimientoCaja entity);
    Task<bool> UpdateAsync(MovimientoCaja entity);
    Task<bool> DeleteAsync(int id);
    Task<MovimientoCaja> GetAsync(int id);
    Task<IEnumerable<MovimientoCaja>> GetAllAsync();
    Task<IEnumerable<MovimientoCaja>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}