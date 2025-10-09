using Common;
using DTO.PedidoDetalle;

namespace Interface.UseCases;

public interface IPedidoDetalleApplication
{
    #region Metodos sincronos

    Response<bool> Insert(PedidoDetalleDTO dto);
    Response<bool> Update(PedidoDetalleDTO dto);
    Response<bool> Delete(int id);
    Response<PedidoDetalleDTO> Get(int id);
    Response<IEnumerable<PedidoDetalleDTO>> GetAll();
    ResponsePagination<IEnumerable<PedidoDetalleDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(PedidoDetalleDTO dto);
    Task<Response<bool>> UpdateAsync(PedidoDetalleDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<PedidoDetalleDTO>> GetAsync(int id);
    Task<Response<IEnumerable<PedidoDetalleDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<PedidoDetalleDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}