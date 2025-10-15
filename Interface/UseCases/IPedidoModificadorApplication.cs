using Common;
using DTO.PedidoModificador;

namespace Interface.UseCases;

public interface IPedidoModificadorApplication
{
    #region Metodos sincronos

    Response<bool> Insert(PedidoModificadorDTO dto);
    Response<bool> Update(PedidoModificadorDTO dto);
    Response<bool> Delete(int id);
    Response<PedidoModificadorDTO> Get(int id);
    Response<IEnumerable<PedidoModificadorDTO>> GetAll();
    ResponsePagination<IEnumerable<PedidoModificadorDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(PedidoModificadorDTO dto);
    Task<Response<bool>> UpdateAsync(PedidoModificadorDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<PedidoModificadorDTO>> GetAsync(int id);
    Task<Response<IEnumerable<PedidoModificadorDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<PedidoModificadorDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}