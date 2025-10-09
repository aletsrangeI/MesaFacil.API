using Common;
using DTO.PedidoAsiento;

namespace Interface.UseCases;

public interface IPedidoAsientoApplication
{
    #region Metodos sincronos

    Response<bool> Insert(PedidoAsientoDTO dto);
    Response<bool> Update(PedidoAsientoDTO dto);
    Response<bool> Delete(int id);
    Response<PedidoAsientoDTO> Get(int id);
    Response<IEnumerable<PedidoAsientoDTO>> GetAll();
    ResponsePagination<IEnumerable<PedidoAsientoDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(PedidoAsientoDTO dto);
    Task<Response<bool>> UpdateAsync(PedidoAsientoDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<PedidoAsientoDTO>> GetAsync(int id);
    Task<Response<IEnumerable<PedidoAsientoDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<PedidoAsientoDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}