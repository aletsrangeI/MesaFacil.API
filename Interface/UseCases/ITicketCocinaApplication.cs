using Common;
using DTO.TicketCocina;

namespace Interface.UseCases;

public interface ITicketCocinaApplication
{
    #region Metodos sincronos

    Response<int> Insert(TicketCocinaDTO dto);
    Response<bool> Update(TicketCocinaDTO dto);
    Response<bool> Delete(int id);
    Response<TicketCocinaDTO> Get(int id);
    Response<IEnumerable<TicketCocinaDTO>> GetAll();
    ResponsePagination<IEnumerable<TicketCocinaDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<int>> InsertAsync(TicketCocinaDTO dto);
    Task<Response<bool>> UpdateAsync(TicketCocinaDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<TicketCocinaDTO>> GetAsync(int id);
    Task<Response<IEnumerable<TicketCocinaDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<TicketCocinaDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}