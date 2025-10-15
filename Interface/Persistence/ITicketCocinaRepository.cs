using Domain.Entities;

namespace Interface.Persistence;

public interface ITicketCocinaRepository
{
    #region Metodos sincronos
    bool Insert(TicketCocina entity);
    bool Update(TicketCocina entity);
    bool Delete(int id);
    TicketCocina Get(int id);
    IEnumerable<TicketCocina> GetAll();
    IEnumerable<TicketCocina> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(TicketCocina entity);
    Task<bool> UpdateAsync(TicketCocina entity);
    Task<bool> DeleteAsync(int id);
    Task<TicketCocina> GetAsync(int id);
    Task<IEnumerable<TicketCocina>> GetAllAsync();
    Task<IEnumerable<TicketCocina>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}