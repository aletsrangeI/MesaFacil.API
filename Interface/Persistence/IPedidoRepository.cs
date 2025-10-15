using Domain.Entities;

namespace Interface.Persistence;

public interface IPedidoRepository
{
    #region Metodos sincronos
    bool Insert(Pedido entity);
    bool Update(Pedido entity);
    bool Delete(int id);
    Pedido Get(int id);
    IEnumerable<Pedido> GetAll();
    IEnumerable<Pedido> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(Pedido entity);
    Task<bool> UpdateAsync(Pedido entity);
    Task<bool> DeleteAsync(int id);
    Task<Pedido> GetAsync(int id);
    Task<IEnumerable<Pedido>> GetAllAsync();
    Task<IEnumerable<Pedido>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}