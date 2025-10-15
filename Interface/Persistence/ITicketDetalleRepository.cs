using Domain.Entities;

namespace Interface.Persistence;

public interface ITicketDetalleRepository
{
    #region Metodos sincronos
    bool Insert(TicketDetalle entity);
    bool Update(TicketDetalle entity);
    bool Delete(int id);
    TicketDetalle Get(int id);
    IEnumerable<TicketDetalle> GetAll();
    IEnumerable<TicketDetalle> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(TicketDetalle entity);
    Task<bool> UpdateAsync(TicketDetalle entity);
    Task<bool> DeleteAsync(int id);
    Task<TicketDetalle> GetAsync(int id);
    Task<IEnumerable<TicketDetalle>> GetAllAsync();
    Task<IEnumerable<TicketDetalle>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}