using Common;
using DTO.EstacionCocina;

namespace Interface.UseCases;

public interface IEstacionCocinaApplication
{
    #region Metodos sincronos

    Response<bool> Insert(EstacionCocinaDTO dto);
    Response<bool> Update(EstacionCocinaDTO dto);
    Response<bool> Delete(int id);
    Response<EstacionCocinaDTO> Get(int id);
    Response<IEnumerable<EstacionCocinaDTO>> GetAll();
    ResponsePagination<IEnumerable<EstacionCocinaDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(EstacionCocinaDTO dto);
    Task<Response<bool>> UpdateAsync(EstacionCocinaDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<EstacionCocinaDTO>> GetAsync(int id);
    Task<Response<IEnumerable<EstacionCocinaDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<EstacionCocinaDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}