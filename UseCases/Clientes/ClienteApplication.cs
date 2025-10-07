using AutoMapper;
using Common;
using Domain.Entities;
using DTO.Cliente;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.Clientes;

public class ClienteApplication : IClienteApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ClienteDTOValidator _validationRules;
    private readonly IAppLogger<ClienteApplication> _logger;

    public ClienteApplication(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ClienteDTOValidator validationRules,
        IAppLogger<ClienteApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(ClienteDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Cliente>(dto);
            response.Data = _unitOfWork.Clientes.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Cliente creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<bool> Update(ClienteDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Cliente>(dto);
            response.Data = _unitOfWork.Clientes.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Cliente modificado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<bool> Delete(int id)
    {
        var response = new Response<bool>();
        try
        {
            response.Data = _unitOfWork.Clientes.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Cliente eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<ClienteDTO> Get(int id)
    {
        var response = new Response<ClienteDTO>();
        try
        {
            var entity = _unitOfWork.Clientes.Get(id);
            response.Data = _mapper.Map<ClienteDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Cliente encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<IEnumerable<ClienteDTO>> GetAll()
    {
        var response = new Response<IEnumerable<ClienteDTO>>();
        try
        {
            var list = _unitOfWork.Clientes.GetAll();
            response.Data = _mapper.Map<IEnumerable<ClienteDTO>>(list);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public ResponsePagination<IEnumerable<ClienteDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<ClienteDTO>>();
        try
        {
            var list = _unitOfWork.Clientes.GetAllWithPagination(page, pageSize);

            if (list != null)
            {
                response.Data = _mapper.Map<IEnumerable<ClienteDTO>>(list);
                response.isSuccess = true;
                response.Message = "Clientes encontrados";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<int> Count()
    {
        var response = new Response<int>();
        try
        {
            response.Data = _unitOfWork.Clientes.Count();
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    #endregion

    #region Metodos asincronos

    public async Task<Response<bool>> InsertAsync(ClienteDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Cliente>(dto);
            response.Data = await _unitOfWork.Clientes.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Cliente creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<bool>> UpdateAsync(ClienteDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Cliente>(dto);
            response.Data = await _unitOfWork.Clientes.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Cliente modificado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<bool>> DeleteAsync(int id)
    {
        var response = new Response<bool>();
        try
        {
            response.Data = await _unitOfWork.Clientes.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Cliente eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<ClienteDTO>> GetAsync(int id)
    {
        var response = new Response<ClienteDTO>();
        try
        {
            var entity = await _unitOfWork.Clientes.GetAsync(id);
            response.Data = _mapper.Map<ClienteDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Cliente encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<IEnumerable<ClienteDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<ClienteDTO>>();
        try
        {
            var list = await _unitOfWork.Clientes.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<ClienteDTO>>(list);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<ResponsePagination<IEnumerable<ClienteDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<ClienteDTO>>();
        try
        {
            var list = await _unitOfWork.Clientes.GetAllWithPaginationAsync(page, pageSize);

            if (list != null)
            {
                response.Data = _mapper.Map<IEnumerable<ClienteDTO>>(list);
                response.isSuccess = true;
                response.Message = "Clientes encontrados";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<int>> CountAsync()
    {
        var response = new Response<int>();
        try
        {
            response.Data = await _unitOfWork.Clientes.CountAsync();
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    #endregion
}