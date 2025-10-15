using Common;
using DTO.Sucursal;

namespace Interface.UseCases;

public interface ISucursalApplication
{
    #region Metodos sincronos

    Response<bool> Insert(SucursalDTO dto);
    Response<bool> Update(SucursalDTO dto);
    Response<bool> Delete(int id);
    Response<SucursalDTO> Get(int id);
    Response<IEnumerable<SucursalDTO>> GetAll();
    ResponsePagination<IEnumerable<SucursalDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(SucursalDTO dto);
    Task<Response<bool>> UpdateAsync(SucursalDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<SucursalDTO>> GetAsync(int id);
    Task<Response<IEnumerable<SucursalDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<SucursalDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}