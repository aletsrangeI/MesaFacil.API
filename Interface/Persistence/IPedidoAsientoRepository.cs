using Domain.Entities;

namespace Interface.Persistence;

public interface IPedidoAsientoRepository
{
    #region Metodos sincronos
    bool Insert(PedidoAsiento entity);
    bool Update(PedidoAsiento entity);
    bool Delete(Guid id);
    PedidoAsiento Get(Guid id);
    IEnumerable<PedidoAsiento> GetAll();
    IEnumerable<PedidoAsiento> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(PedidoAsiento entity);
    Task<bool> UpdateAsync(PedidoAsiento entity);
    Task<bool> DeleteAsync(Guid id);
    Task<PedidoAsiento> GetAsync(Guid id);
    Task<IEnumerable<PedidoAsiento>> GetAllAsync();
    Task<IEnumerable<PedidoAsiento>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}