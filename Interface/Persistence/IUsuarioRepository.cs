using Domain.Entities;

namespace Interface.Persistence;

public interface IUsuarioRepository
{
    /// <summary>
    /// Obtiene un usuario por correo o username (si tu modelo usa username después podrás extender).
    /// Incluye Usuarioes->Rol y Credenciales.
    /// </summary>
    
    #region Metodos sincronos
    bool Insert(Usuario entity);
    bool Update(Usuario entity);
    bool Delete(int id);
    Usuario Get(int id);
    IEnumerable<Usuario> GetAll();
    IEnumerable<Usuario> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(Usuario entity);
    Task<bool> UpdateAsync(Usuario entity);
    Task<bool> DeleteAsync(int id);
    Task<Usuario> GetAsync(int id);
    Task<IEnumerable<Usuario>> GetAllAsync();
    Task<IEnumerable<Usuario>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
    
    
    Task<Usuario?> GetByCorreoWithRolesAndCredentialsAsync(string correo, CancellationToken ct);

    Task<bool> HasOpenTurnoAsync(int idUsuario, CancellationToken ct);
}