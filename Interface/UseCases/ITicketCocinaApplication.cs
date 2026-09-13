using Common;
using DTO.TicketCocina;

namespace Interface.UseCases;

public interface ITicketCocinaApplication
{
    #region Metodos sincronos

    Response<Guid> Insert(TicketCocinaDTO dto);
    Response<bool> Update(TicketCocinaDTO dto);
    Response<bool> Delete(Guid id);
    Response<TicketCocinaDTO> Get(Guid id);
    Response<IEnumerable<TicketCocinaDTO>> GetAll();
    ResponsePagination<IEnumerable<TicketCocinaDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<Guid>> InsertAsync(TicketCocinaDTO dto);
    Task<Response<bool>> UpdateAsync(TicketCocinaDTO dto);
    Task<Response<bool>> DeleteAsync(Guid id);
    Task<Response<TicketCocinaDTO>> GetAsync(Guid id);
    Task<Response<IEnumerable<TicketCocinaDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<TicketCocinaDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}