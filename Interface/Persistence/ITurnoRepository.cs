using Domain.Entities;

namespace Interface.Persistence;

public interface ITurnoRepository
{
    #region Metodos sincronos
    bool Insert(Turno entity);
    bool Update(Turno entity);
    bool Delete(int id);
    Turno Get(int id);
    IEnumerable<Turno> GetAll();
    IEnumerable<Turno> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(Turno entity);
    Task<bool> UpdateAsync(Turno entity);
    Task<bool> DeleteAsync(int id);
    Task<Turno> GetAsync(int id);
    Task<IEnumerable<Turno>> GetAllAsync();
    Task<IEnumerable<Turno>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}