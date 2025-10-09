using Common;
using DTO.TicketDetalle;

namespace Interface.UseCases;

public interface ITicketDetalleApplication
{
    #region Metodos sincronos

    Response<bool> Insert(TicketDetalleDTO dto);
    Response<bool> Update(TicketDetalleDTO dto);
    Response<bool> Delete(int id);
    Response<TicketDetalleDTO> Get(int id);
    Response<IEnumerable<TicketDetalleDTO>> GetAll();
    ResponsePagination<IEnumerable<TicketDetalleDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(TicketDetalleDTO dto);
    Task<Response<bool>> UpdateAsync(TicketDetalleDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<TicketDetalleDTO>> GetAsync(int id);
    Task<Response<IEnumerable<TicketDetalleDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<TicketDetalleDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}