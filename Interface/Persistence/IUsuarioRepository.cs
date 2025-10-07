using Domain.Entities;

namespace Interface.Persistence;

public interface IUsuarioRepository
{
    /// <summary>
    /// Obtiene un usuario por correo o username (si tu modelo usa username después podrás extender).
    /// Incluye UsuarioRoles->Rol y Credenciales.
    /// </summary>
    Task<Usuario?> GetByCorreoWithRolesAndCredentialsAsync(string correo, CancellationToken ct);

    Task<bool> HasOpenTurnoAsync(int idUsuario, CancellationToken ct);
}