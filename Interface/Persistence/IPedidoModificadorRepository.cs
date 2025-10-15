using Domain.Entities;

namespace Interface.Persistence;

public interface IPedidoModificadorRepository
{
    #region Metodos sincronos
    bool Insert(PedidoModificador entity);
    bool Update(PedidoModificador entity);
    bool Delete(int id);
    PedidoModificador Get(int id);
    IEnumerable<PedidoModificador> GetAll();
    IEnumerable<PedidoModificador> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(PedidoModificador entity);
    Task<bool> UpdateAsync(PedidoModificador entity);
    Task<bool> DeleteAsync(int id);
    Task<PedidoModificador> GetAsync(int id);
    Task<IEnumerable<PedidoModificador>> GetAllAsync();
    Task<IEnumerable<PedidoModificador>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}