using Common;
using DTO.EventoPedido;

namespace Interface.UseCases;

public interface IEventoPedidoApplication
{
    #region Metodos sincronos

    Response<bool> Insert(EventoPedidoDTO dto);
    Response<bool> Update(EventoPedidoDTO dto);
    Response<bool> Delete(int id);
    Response<EventoPedidoDTO> Get(int id);
    Response<IEnumerable<EventoPedidoDTO>> GetAll();
    ResponsePagination<IEnumerable<EventoPedidoDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(EventoPedidoDTO dto);
    Task<Response<bool>> UpdateAsync(EventoPedidoDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<EventoPedidoDTO>> GetAsync(int id);
    Task<Response<IEnumerable<EventoPedidoDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<EventoPedidoDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}