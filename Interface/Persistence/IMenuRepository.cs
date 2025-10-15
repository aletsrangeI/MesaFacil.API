using Domain.Entities;

namespace Interface.Persistence;

public interface IMenuRepository
{
    #region Metodos sincronos
    bool Insert(Menu entity);
    bool Update(Menu entity);
    bool Delete(int id);
    Menu Get(int id);
    IEnumerable<Menu> GetAll();
    IEnumerable<Menu> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(Menu entity);
    Task<bool> UpdateAsync(Menu entity);
    Task<bool> DeleteAsync(int id);
    Task<Menu> GetAsync(int id);
    Task<IEnumerable<Menu>> GetAllAsync();
    Task<IEnumerable<Menu>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}