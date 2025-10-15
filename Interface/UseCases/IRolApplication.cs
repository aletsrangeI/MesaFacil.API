using Common;
using DTO.Rol;

namespace Interface.UseCases;

public interface IRolApplication
{
    #region Metodos sincronos

    Response<bool> Insert(RolDTO dto);
    Response<bool> Update(RolDTO dto);
    Response<bool> Delete(int id);
    Response<RolDTO> Get(int id);
    Response<IEnumerable<RolDTO>> GetAll();
    ResponsePagination<IEnumerable<RolDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(RolDTO dto);
    Task<Response<bool>> UpdateAsync(RolDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<RolDTO>> GetAsync(int id);
    Task<Response<IEnumerable<RolDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<RolDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}