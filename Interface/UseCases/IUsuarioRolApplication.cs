using Common;
using DTO.UsuarioRol;

namespace Interface.UseCases;

public interface IUsuarioRolApplication
{
    #region Metodos sincronos

    Response<bool> Insert(UsuarioRolDTO dto);
    Response<bool> Update(UsuarioRolDTO dto);
    Response<bool> Delete(int id);
    Response<UsuarioRolDTO> Get(int id);
    Response<IEnumerable<UsuarioRolDTO>> GetAll();
    ResponsePagination<IEnumerable<UsuarioRolDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(UsuarioRolDTO dto);
    Task<Response<bool>> UpdateAsync(UsuarioRolDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<UsuarioRolDTO>> GetAsync(int id);
    Task<Response<IEnumerable<UsuarioRolDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<UsuarioRolDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}