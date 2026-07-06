using Common;
using DTO.Usuario;

namespace Interface.UseCases;

public interface IUsuarioApplication
{
    #region Metodos sincronos

    Response<bool> Insert(UsuarioDTO dto);
    Response<bool> Update(UsuarioDTO dto);
    Response<bool> Delete(int id);
    Response<UsuarioDTO> Get(int id);
    Response<IEnumerable<UsuarioDTO>> GetAll();
    ResponsePagination<IEnumerable<UsuarioDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(UsuarioDTO dto);
    Task<Response<bool>> UpdateAsync(UsuarioDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<UsuarioDTO>> GetAsync(int id);
    Task<Response<IEnumerable<UsuarioDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<UsuarioDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion

    #region Metodos de Seguridad y Operativos

    Task<Response<UsuarioDTO?>> GetByCorreoWithRolesAndCredentialsAsync(string correo, CancellationToken ct);
    Task<Response<UsuarioDTO?>> GetByUserOrEmailWithAuthGraphAsync(string userOrEmail, CancellationToken ct);
    Task<Response<IReadOnlyList<string>>> GetRoleNamesAsync(int usuarioId, CancellationToken ct);
    Task<Response<IReadOnlyList<string>>> GetAccesoPathsByUsuarioIdAsync(int usuarioId, CancellationToken ct);
    Task<Response<object?>> GetPasswordCredentialAsync(int usuarioId, CancellationToken ct); 
    Task<Response<bool>> HasOpenTurnoAsync(int idUsuario, CancellationToken ct);
    Task<Response<List<string>>> GetPermissionKeysByUsuarioIdAsync(int usuarioId, CancellationToken ct);
    Task<Response<string?>> GetPermissionsVersionAsync(int usuarioId, CancellationToken ct);

    #endregion
}