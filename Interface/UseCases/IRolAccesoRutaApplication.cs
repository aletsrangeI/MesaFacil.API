using Common;
using DTO.RolAccesoRuta;

namespace Interface.UseCases;

public interface IRolAccesoRutaApplication
{
    #region Metodos sincronos

    Response<bool> Insert(RolAccesoRutaDTO dto);
    Response<bool> Update(RolAccesoRutaDTO dto);
    Response<bool> Delete(int id);
    Response<RolAccesoRutaDTO> Get(int id);
    Response<IEnumerable<RolAccesoRutaDTO>> GetAll();
    ResponsePagination<IEnumerable<RolAccesoRutaDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(RolAccesoRutaDTO dto);
    Task<Response<bool>> UpdateAsync(RolAccesoRutaDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<RolAccesoRutaDTO>> GetAsync(int id);
    Task<Response<IEnumerable<RolAccesoRutaDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<RolAccesoRutaDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}