using Domain.Entities;

namespace Interface.Persistence;

public interface IEmpresaRepository
{
    #region Metodos sincronos
    bool Insert(Empresa entity);
    bool Update(Empresa entity);
    bool Delete(int id);
    Empresa Get(int id);
    IEnumerable<Empresa> GetAll();
    IEnumerable<Empresa> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(Empresa entity);
    Task<bool> UpdateAsync(Empresa entity);
    Task<bool> DeleteAsync(int id);
    Task<Empresa> GetAsync(int id);
    Task<IEnumerable<Empresa>> GetAllAsync();
    Task<IEnumerable<Empresa>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}