using Common;
using DTO.Pedido;

namespace Interface.UseCases;

public interface IPedidoApplication
{
    #region Metodos sincronos

    Response<bool> Insert(PedidoDTO dto);
    Response<bool> Update(PedidoDTO dto);
    Response<bool> Delete(Guid id);
    Response<PedidoDTO> Get(Guid id);
    Response<IEnumerable<PedidoDTO>> GetAll();
    ResponsePagination<IEnumerable<PedidoDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(PedidoDTO dto);
    Task<Response<Guid>> InsertConDetallesAsync(CrearPedidoRequestDTO dto);
    Task<Response<bool>> UpdateAsync(PedidoDTO dto);
    Task<Response<bool>> DeleteAsync(Guid id);
    Task<Response<PedidoDTO>> GetAsync(Guid id);
    Task<Response<IEnumerable<PedidoDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<PedidoDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}