using Domain.Entities;

namespace Interface.Persistence;

public interface IOpcionModificadorRepository
{
    #region Metodos sincronos
    bool Insert(OpcionModificador entity);
    bool Update(OpcionModificador entity);
    bool Delete(int id);
    OpcionModificador Get(int id);
    IEnumerable<OpcionModificador> GetAll();
    IEnumerable<OpcionModificador> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(OpcionModificador entity);
    Task<bool> UpdateAsync(OpcionModificador entity);
    Task<bool> DeleteAsync(int id);
    Task<OpcionModificador> GetAsync(int id);
    Task<IEnumerable<OpcionModificador>> GetAllAsync();
    Task<IEnumerable<OpcionModificador>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}