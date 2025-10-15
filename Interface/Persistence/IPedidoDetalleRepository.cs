using Domain.Entities;

namespace Interface.Persistence;

public interface IPedidoDetalleRepository
{
    #region Metodos sincronos
    bool Insert(PedidoDetalle entity);
    bool Update(PedidoDetalle entity);
    bool Delete(int id);
    PedidoDetalle Get(int id);
    IEnumerable<PedidoDetalle> GetAll();
    IEnumerable<PedidoDetalle> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(PedidoDetalle entity);
    Task<bool> UpdateAsync(PedidoDetalle entity);
    Task<bool> DeleteAsync(int id);
    Task<PedidoDetalle> GetAsync(int id);
    Task<IEnumerable<PedidoDetalle>> GetAllAsync();
    Task<IEnumerable<PedidoDetalle>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}