using Domain.Entities;

namespace Interface.Persistence;

public interface IPrecioRepository
{
    #region Metodos sincronos
    bool Insert(Precio entity);
    bool Update(Precio entity);
    bool Delete(int id);
    Precio Get(int id);
    IEnumerable<Precio> GetAll();
    IEnumerable<Precio> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(Precio entity);
    Task<bool> UpdateAsync(Precio entity);
    Task<bool> DeleteAsync(int id);
    Task<Precio> GetAsync(int id);
    Task<IEnumerable<Precio>> GetAllAsync();
    Task<IEnumerable<Precio>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}