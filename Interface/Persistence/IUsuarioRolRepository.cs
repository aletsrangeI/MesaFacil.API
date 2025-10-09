using Domain.Entities;

namespace Interface.Persistence;

public interface IUsuarioRolRepository
{
    #region Metodos sincronos
    bool Insert(UsuarioRol entity);
    bool Update(UsuarioRol entity);
    bool Delete(int id);
    UsuarioRol Get(int id);
    IEnumerable<UsuarioRol> GetAll();
    IEnumerable<UsuarioRol> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(UsuarioRol entity);
    Task<bool> UpdateAsync(UsuarioRol entity);
    Task<bool> DeleteAsync(int id);
    Task<UsuarioRol> GetAsync(int id);
    Task<IEnumerable<UsuarioRol>> GetAllAsync();
    Task<IEnumerable<UsuarioRol>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}