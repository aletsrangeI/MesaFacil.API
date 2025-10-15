using AutoMapper;
using Common;
using Domain.Entities;
using DTO.PedidoModificador;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.PedidoModificadores;

public class PedidoModificadorApplication : IPedidoModificadorApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly PedidoModificadorDTOValidator _validationRules;
    private readonly IAppLogger<PedidoModificadorApplication> _logger;

    public PedidoModificadorApplication(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        PedidoModificadorDTOValidator validationRules,
        IAppLogger<PedidoModificadorApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(PedidoModificadorDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<PedidoModificador>(dto);
            response.Data = _unitOfWork.PedidoModificadores.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "PedidoModificador creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(PedidoModificadorDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<PedidoModificador>(dto);
            response.Data = _unitOfWork.PedidoModificadores.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "PedidoModificador modificado correctamente";
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
            response.Data = _unitOfWork.PedidoModificadores.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "PedidoModificador eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<PedidoModificadorDTO> Get(int id)
    {
        var response = new Response<PedidoModificadorDTO>();
        try
        {
            var entity = _unitOfWork.PedidoModificadores.Get(id);
            response.Data = _mapper.Map<PedidoModificadorDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "PedidoModificador encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<PedidoModificadorDTO>> GetAll()
    {
        var response = new Response<IEnumerable<PedidoModificadorDTO>>();
        try
        {
            var list = _unitOfWork.PedidoModificadores.GetAll();
            response.Data = _mapper.Map<IEnumerable<PedidoModificadorDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "PedidoModificador encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<PedidoModificadorDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<PedidoModificadorDTO>>();
        try
        {
            var list = _unitOfWork.PedidoModificadores.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<PedidoModificadorDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "PedidoModificador encontrado";
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
            response.Data = _unitOfWork.PedidoModificadores.Count();
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

    public async Task<Response<bool>> InsertAsync(PedidoModificadorDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<PedidoModificador>(dto);
            response.Data = await _unitOfWork.PedidoModificadores.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "PedidoModificador creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(PedidoModificadorDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<PedidoModificador>(dto);
            response.Data = await _unitOfWork.PedidoModificadores.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "PedidoModificador modificado correctamente";
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
            response.Data = await _unitOfWork.PedidoModificadores.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "PedidoModificador eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<PedidoModificadorDTO>> GetAsync(int id)
    {
        var response = new Response<PedidoModificadorDTO>();
        try
        {
            var entity = await _unitOfWork.PedidoModificadores.GetAsync(id);
            response.Data = _mapper.Map<PedidoModificadorDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "PedidoModificador encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<PedidoModificadorDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<PedidoModificadorDTO>>();
        try
        {
            var list = await _unitOfWork.PedidoModificadores.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<PedidoModificadorDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "PedidoModificador encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<PedidoModificadorDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<PedidoModificadorDTO>>();
        try
        {
            var list = await _unitOfWork.PedidoModificadores.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<PedidoModificadorDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "PedidoModificador encontrado";
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
            response.Data = await _unitOfWork.PedidoModificadores.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "PedidoModificador encontrado";
            }
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