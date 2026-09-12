using Common;
using DTO.TiposPedido;

namespace Interface.UseCases.TiposPedido;

public interface ITipoPedidoApplication
{
    Task<Response<bool>> InsertAsync(TipoPedidoDTO dto);
    Task<Response<bool>> UpdateAsync(TipoPedidoDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<TipoPedidoDTO>> GetAsync(int id);
    Task<Response<IEnumerable<TipoPedidoDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<TipoPedidoDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();
}
