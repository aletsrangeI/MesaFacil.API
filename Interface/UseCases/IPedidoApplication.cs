using Common;
using DTO.Pedido;

namespace Interface.UseCases;

public interface IPedidoApplication
{
    #region Metodos sincronos

    Response<bool> Insert(PedidoDTO dto);
    Response<bool> Update(PedidoDTO dto);
    Response<bool> Delete(int id);
    Response<PedidoDTO> Get(int id);
    Response<IEnumerable<PedidoDTO>> GetAll();
    ResponsePagination<IEnumerable<PedidoDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(PedidoDTO dto);
    Task<Response<int>> InsertConDetallesAsync(CrearPedidoRequestDTO dto);
    Task<Response<bool>> UpdateAsync(PedidoDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<PedidoDTO>> GetAsync(int id);
    Task<Response<IEnumerable<PedidoDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<PedidoDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}