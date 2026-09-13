using Common;
using DTO.PedidoModificador;

namespace Interface.UseCases;

public interface IPedidoModificadorApplication
{
    #region Metodos sincronos

    Response<bool> Insert(PedidoModificadorDTO dto);
    Response<bool> Update(PedidoModificadorDTO dto);
    Response<bool> Delete(Guid id);
    Response<PedidoModificadorDTO> Get(Guid id);
    Response<IEnumerable<PedidoModificadorDTO>> GetAll();
    ResponsePagination<IEnumerable<PedidoModificadorDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(PedidoModificadorDTO dto);
    Task<Response<bool>> UpdateAsync(PedidoModificadorDTO dto);
    Task<Response<bool>> DeleteAsync(Guid id);
    Task<Response<PedidoModificadorDTO>> GetAsync(Guid id);
    Task<Response<IEnumerable<PedidoModificadorDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<PedidoModificadorDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}