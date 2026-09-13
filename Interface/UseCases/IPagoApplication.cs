using Common;
using DTO.Pago;

namespace Interface.UseCases;

public interface IPagoApplication
{
    #region Metodos sincronos

    Response<bool> Insert(PagoDTO dto);
    Response<bool> Update(PagoDTO dto);
    Response<bool> Delete(Guid id);
    Response<PagoDTO> Get(Guid id);
    Response<IEnumerable<PagoDTO>> GetAll();
    ResponsePagination<IEnumerable<PagoDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(PagoDTO dto);
    Task<Response<bool>> UpdateAsync(PagoDTO dto);
    Task<Response<bool>> DeleteAsync(Guid id);
    Task<Response<PagoDTO>> GetAsync(Guid id);
    Task<Response<IEnumerable<PagoDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<PagoDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    Task<Response<bool>> RegistrarPagoAsync(DTO.Pago.RegistrarPagoDTO dto);
    Response<bool> RegistrarPago(DTO.Pago.RegistrarPagoDTO dto);

    #endregion
}