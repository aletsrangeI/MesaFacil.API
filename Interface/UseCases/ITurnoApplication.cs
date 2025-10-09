using Common;
using DTO.Turno;

namespace Interface.UseCases;

public interface ITurnoApplication
{
    #region Metodos sincronos

    Response<bool> Insert(TurnoDTO dto);
    Response<bool> Update(TurnoDTO dto);
    Response<bool> Delete(int id);
    Response<TurnoDTO> Get(int id);
    Response<IEnumerable<TurnoDTO>> GetAll();
    ResponsePagination<IEnumerable<TurnoDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(TurnoDTO dto);
    Task<Response<bool>> UpdateAsync(TurnoDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<TurnoDTO>> GetAsync(int id);
    Task<Response<IEnumerable<TurnoDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<TurnoDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}