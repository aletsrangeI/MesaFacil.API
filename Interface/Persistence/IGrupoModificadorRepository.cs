using Domain.Entities;

namespace Interface.Persistence;

public interface IGrupoModificadorRepository
{
    #region Metodos sincronos
    bool Insert(GrupoModificador entity);
    bool Update(GrupoModificador entity);
    bool Delete(int id);
    GrupoModificador Get(int id);
    IEnumerable<GrupoModificador> GetAll();
    IEnumerable<GrupoModificador> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(GrupoModificador entity);
    Task<bool> UpdateAsync(GrupoModificador entity);
    Task<bool> DeleteAsync(int id);
    Task<GrupoModificador> GetAsync(int id);
    Task<IEnumerable<GrupoModificador>> GetAllAsync();
    Task<IEnumerable<GrupoModificador>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}