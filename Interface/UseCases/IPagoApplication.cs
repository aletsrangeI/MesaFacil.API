using Common;
using DTO.Pago;

namespace Interface.UseCases;

public interface IPagoApplication
{
    #region Metodos sincronos

    Response<bool> Insert(PagoDTO dto);
    Response<bool> Update(PagoDTO dto);
    Response<bool> Delete(int id);
    Response<PagoDTO> Get(int id);
    Response<IEnumerable<PagoDTO>> GetAll();
    ResponsePagination<IEnumerable<PagoDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(PagoDTO dto);
    Task<Response<bool>> UpdateAsync(PagoDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<PagoDTO>> GetAsync(int id);
    Task<Response<IEnumerable<PagoDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<PagoDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    Task<Response<bool>> RegistrarPagoAsync(DTO.Pago.RegistrarPagoDTO dto);
    Response<bool> RegistrarPago(DTO.Pago.RegistrarPagoDTO dto);

    #endregion
}