using Domain.Entities;

namespace Interface.Persistence;

public interface IUsuarioRepository
{
    #region Metodos estándar (Sincronos y Asincronos)
    Task<bool> InsertAsync(Usuario entity);
    Task<bool> UpdateAsync(Usuario entity);
    Task<bool> DeleteAsync(int id);
    Task<Usuario> GetAsync(int id);
    Task<IEnumerable<Usuario>> GetAllAsync();
    
    
    #endregion

    // Búsqueda para login/perfil
    Task<Usuario?> GetByCorreoWithRolesAndCredentialsAsync(string correo, CancellationToken ct);
    Task<Usuario?> GetByUserOrEmailWithAuthGraphAsync(string userOrEmail, CancellationToken ct);

    // Seguridad y RBAC
    Task<IReadOnlyList<string>> GetRoleNamesAsync(int usuarioId, CancellationToken ct);
    Task<IReadOnlyList<string>> GetAccesoPathsByUsuarioIdAsync(int usuarioId, CancellationToken ct);
    Task<List<string>> GetPermissionKeysByUsuarioIdAsync(int usuarioId, CancellationToken ct);
    Task<string?> GetPermissionsVersionAsync(int usuarioId, CancellationToken ct);

    // Credenciales
    Task<Credencial?> GetPasswordCredentialAsync(int usuarioId, CancellationToken ct);

    // Operativo
    Task<bool> HasOpenTurnoAsync(int idUsuario, CancellationToken ct);
}