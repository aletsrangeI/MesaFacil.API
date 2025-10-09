using Common;
using DTO.Mesa;

namespace Interface.UseCases;

public interface IMesaApplication
{
    #region Metodos sincronos

    Response<bool> Insert(MesaDTO dto);
    Response<bool> Update(MesaDTO dto);
    Response<bool> Delete(int id);
    Response<MesaDTO> Get(int id);
    Response<IEnumerable<MesaDTO>> GetAll();
    ResponsePagination<IEnumerable<MesaDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(MesaDTO dto);
    Task<Response<bool>> UpdateAsync(MesaDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<MesaDTO>> GetAsync(int id);
    Task<Response<IEnumerable<MesaDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<MesaDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}