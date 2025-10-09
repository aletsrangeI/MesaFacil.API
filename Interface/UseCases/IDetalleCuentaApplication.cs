using Common;
using DTO.DetalleCuenta;

namespace Interface.UseCases;

public interface IDetalleCuentaApplication
{
    #region Metodos sincronos

    Response<bool> Insert(DetalleCuentaDTO dto);
    Response<bool> Update(DetalleCuentaDTO dto);
    Response<bool> Delete(int id);
    Response<DetalleCuentaDTO> Get(int id);
    Response<IEnumerable<DetalleCuentaDTO>> GetAll();
    ResponsePagination<IEnumerable<DetalleCuentaDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(DetalleCuentaDTO dto);
    Task<Response<bool>> UpdateAsync(DetalleCuentaDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<DetalleCuentaDTO>> GetAsync(int id);
    Task<Response<IEnumerable<DetalleCuentaDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<DetalleCuentaDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}