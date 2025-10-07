using Domain.Entities;

namespace Interface.Persistence;

public interface IClienteRepository
{
    #region Metodos sincronos
    bool Insert(Cliente entity);
    bool Update(Cliente entity);
    bool Delete(int id);
    Cliente Get(int id);
    IEnumerable<Cliente> GetAll();
    IEnumerable<Cliente> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(Cliente entity);
    Task<bool> UpdateAsync(Cliente entity);
    Task<bool> DeleteAsync(int id);
    Task<Cliente> GetAsync(int id);
    Task<IEnumerable<Cliente>> GetAllAsync();
    Task<IEnumerable<Cliente>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}