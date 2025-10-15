using Domain.Entities;

namespace Interface.Persistence;

public interface IEventoPedidoRepository
{
    #region Metodos sincronos
    bool Insert(EventoPedido entity);
    bool Update(EventoPedido entity);
    bool Delete(int id);
    EventoPedido Get(int id);
    IEnumerable<EventoPedido> GetAll();
    IEnumerable<EventoPedido> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(EventoPedido entity);
    Task<bool> UpdateAsync(EventoPedido entity);
    Task<bool> DeleteAsync(int id);
    Task<EventoPedido> GetAsync(int id);
    Task<IEnumerable<EventoPedido>> GetAllAsync();
    Task<IEnumerable<EventoPedido>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}