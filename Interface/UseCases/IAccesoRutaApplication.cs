using Common;
using DTO.AccesoRuta;

namespace Interface.UseCases;

public interface IAccesoRutaApplication
{
    #region Metodos sincronos

    Response<bool> Insert(AccesoRutaDTO dto);
    Response<bool> Update(AccesoRutaDTO dto);
    Response<bool> Delete(int id);
    Response<AccesoRutaDTO> Get(int id);
    Response<IEnumerable<AccesoRutaDTO>> GetAll();
    ResponsePagination<IEnumerable<AccesoRutaDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(AccesoRutaDTO dto);
    Task<Response<bool>> UpdateAsync(AccesoRutaDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<AccesoRutaDTO>> GetAsync(int id);
    Task<Response<IEnumerable<AccesoRutaDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<AccesoRutaDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}