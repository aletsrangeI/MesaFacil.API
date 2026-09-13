using Domain.Entities;

namespace Interface.Persistence;

public interface IPagoRepository
{
    #region Metodos sincronos
    bool Insert(Pago entity);
    bool Update(Pago entity);
    bool Delete(Guid id);
    Pago Get(Guid id);
    IEnumerable<Pago> GetAll();
    IEnumerable<Pago> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(Pago entity);
    Task<bool> UpdateAsync(Pago entity);
    Task<bool> DeleteAsync(Guid id);
    Task<Pago> GetAsync(Guid id);
    Task<IEnumerable<Pago>> GetAllAsync();
    Task<IEnumerable<Pago>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}