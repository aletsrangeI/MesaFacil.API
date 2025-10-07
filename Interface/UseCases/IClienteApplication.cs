using Common;
using DTO.Cliente;

namespace Interface.UseCases;

public interface IClienteApplication
{
    #region Metodos sincronos

    Response<bool> Insert(ClienteDTO dto);
    Response<bool> Update(ClienteDTO dto);
    Response<bool> Delete(int id);
    Response<ClienteDTO> Get(int id);
    Response<IEnumerable<ClienteDTO>> GetAll();
    ResponsePagination<IEnumerable<ClienteDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(ClienteDTO dto);
    Task<Response<bool>> UpdateAsync(ClienteDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<ClienteDTO>> GetAsync(int id);
    Task<Response<IEnumerable<ClienteDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<ClienteDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}