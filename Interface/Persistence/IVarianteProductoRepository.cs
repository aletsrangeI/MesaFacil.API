using Domain.Entities;

namespace Interface.Persistence;

public interface IVarianteProductoRepository
{
    #region Metodos sincronos
    bool Insert(VarianteProducto entity);
    bool Update(VarianteProducto entity);
    bool Delete(int id);
    VarianteProducto Get(int id);
    IEnumerable<VarianteProducto> GetAll();
    IEnumerable<VarianteProducto> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(VarianteProducto entity);
    Task<bool> UpdateAsync(VarianteProducto entity);
    Task<bool> DeleteAsync(int id);
    Task<VarianteProducto> GetAsync(int id);
    Task<IEnumerable<VarianteProducto>> GetAllAsync();
    Task<IEnumerable<VarianteProducto>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}