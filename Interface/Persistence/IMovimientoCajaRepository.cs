using Domain.Entities;

namespace Interface.Persistence;

public interface IMovimientoCajaRepository
{
    #region Metodos sincronos
    bool Insert(MovimientoCaja entity);
    bool Update(MovimientoCaja entity);
    bool Delete(Guid id);
    MovimientoCaja Get(Guid id);
    IEnumerable<MovimientoCaja> GetAll();
    IEnumerable<MovimientoCaja> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(MovimientoCaja entity);
    Task<bool> UpdateAsync(MovimientoCaja entity);
    Task<bool> DeleteAsync(Guid id);
    Task<MovimientoCaja> GetAsync(Guid id);
    Task<IEnumerable<MovimientoCaja>> GetAllAsync();
    Task<IEnumerable<MovimientoCaja>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}