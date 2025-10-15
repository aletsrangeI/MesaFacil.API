using Domain.Entities;

namespace Interface.Persistence;

public interface IProductoRepository
{
    #region Metodos sincronos
    bool Insert(Producto entity);
    bool Update(Producto entity);
    bool Delete(int id);
    Producto Get(int id);
    IEnumerable<Producto> GetAll();
    IEnumerable<Producto> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(Producto entity);
    Task<bool> UpdateAsync(Producto entity);
    Task<bool> DeleteAsync(int id);
    Task<Producto> GetAsync(int id);
    Task<IEnumerable<Producto>> GetAllAsync();
    Task<IEnumerable<Producto>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}