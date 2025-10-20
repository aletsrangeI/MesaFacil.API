using Domain.Entities;

namespace Interface.Persistence;

public interface IUsuarioRepository
{
    
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
    
    /// <summary>
    /// Devuelve el usuario por userOrEmail (usuario o correo), incluyendo
    /// lo necesario para autenticación (p.ej. relación a Credencial de password).
    /// No es obligatorio cargar roles aquí si prefieres separarlo.
    /// </summary>
    Task<Usuario?> GetByUserOrEmailWithAuthGraphAsync(string userOrEmail, CancellationToken ct);

    /// <summary>
    /// Nombres de roles asignados al usuario (p.ej. ["Admin","Mesero"]).
    /// </summary>
    Task<IReadOnlyList<string>> GetRoleNamesAsync(int usuarioId, CancellationToken ct);

    /// <summary>
    /// Paths de accesos permitidos a partir de los roles del usuario (p.ej. ["/", "/admin"]).
    /// Implementación típica: Usuario -> UsuarioRol -> Rol -> RolAccesoRuta -> AccesoRuta.Path
    /// </summary>
    Task<IReadOnlyList<string>> GetAccesoPathsByUsuarioIdAsync(int usuarioId, CancellationToken ct);

    /// <summary>
    /// (Opcional) Obtiene la credencial de password del usuario si manejas tipos de credencial.
    /// Útil si quieres validar hash/salt en capa de aplicación.
    /// </summary>
    Task<Credencial?> GetPasswordCredentialAsync(int usuarioId, CancellationToken ct);

    Task<List<string>> GetPermissionKeysByUsuarioIdAsync(int usuarioId, CancellationToken ct);
    
    Task<string?> GetPermissionsVersionAsync(int usuarioId, CancellationToken ct);
}