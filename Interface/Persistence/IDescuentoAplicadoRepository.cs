using Domain.Entities;

namespace Interface.Persistence;

public interface IDescuentoAplicadoRepository
{
    #region Metodos sincronos
    bool Insert(DescuentoAplicado entity);
    bool Update(DescuentoAplicado entity);
    bool Delete(int id);
    DescuentoAplicado Get(int id);
    IEnumerable<DescuentoAplicado> GetAll();
    IEnumerable<DescuentoAplicado> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(DescuentoAplicado entity);
    Task<bool> UpdateAsync(DescuentoAplicado entity);
    Task<bool> DeleteAsync(int id);
    Task<DescuentoAplicado> GetAsync(int id);
    Task<IEnumerable<DescuentoAplicado>> GetAllAsync();
    Task<IEnumerable<DescuentoAplicado>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}