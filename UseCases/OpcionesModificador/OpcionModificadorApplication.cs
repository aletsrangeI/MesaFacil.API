using Interface.Mapping;
using Common;
using Domain.Entities;
using DTO.OpcionModificador;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.OpcionesModificador;

public class OpcionModificadorApplication : IOpcionModificadorApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppMapper _mapper;
    private readonly OpcionModificadorDTOValidator _validationRules;
    private readonly IAppLogger<OpcionModificadorApplication> _logger;

    public OpcionModificadorApplication(
        IUnitOfWork unitOfWork,
        IAppMapper mapper,
        OpcionModificadorDTOValidator validationRules,
        IAppLogger<OpcionModificadorApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(OpcionModificadorDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<OpcionModificador>(dto);
            response.Data = _unitOfWork.OpcionesModificador.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "OpcionModificador creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(OpcionModificadorDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<OpcionModificador>(dto);
            response.Data = _unitOfWork.OpcionesModificador.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "OpcionModificador modificado correctamente";
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
            response.Data = _unitOfWork.OpcionesModificador.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "OpcionModificador eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<OpcionModificadorDTO> Get(int id)
    {
        var response = new Response<OpcionModificadorDTO>();
        try
        {
            var entity = _unitOfWork.OpcionesModificador.Get(id);
            response.Data = _mapper.Map<OpcionModificadorDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "OpcionModificador encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<OpcionModificadorDTO>> GetAll()
    {
        var response = new Response<IEnumerable<OpcionModificadorDTO>>();
        try
        {
            var list = _unitOfWork.OpcionesModificador.GetAll();
            response.Data = _mapper.Map<IEnumerable<OpcionModificadorDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "OpcionModificador encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<OpcionModificadorDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<OpcionModificadorDTO>>();
        try
        {
            var list = _unitOfWork.OpcionesModificador.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<OpcionModificadorDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "OpcionModificador encontrado";
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
            response.Data = _unitOfWork.OpcionesModificador.Count();
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

    public async Task<Response<bool>> InsertAsync(OpcionModificadorDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<OpcionModificador>(dto);
            response.Data = await _unitOfWork.OpcionesModificador.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "OpcionModificador creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(OpcionModificadorDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<OpcionModificador>(dto);
            response.Data = await _unitOfWork.OpcionesModificador.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "OpcionModificador modificado correctamente";
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
            response.Data = await _unitOfWork.OpcionesModificador.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "OpcionModificador eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<OpcionModificadorDTO>> GetAsync(int id)
    {
        var response = new Response<OpcionModificadorDTO>();
        try
        {
            var entity = await _unitOfWork.OpcionesModificador.GetAsync(id);
            response.Data = _mapper.Map<OpcionModificadorDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "OpcionModificador encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<OpcionModificadorDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<OpcionModificadorDTO>>();
        try
        {
            var list = await _unitOfWork.OpcionesModificador.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<OpcionModificadorDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "OpcionModificador encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<OpcionModificadorDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<OpcionModificadorDTO>>();
        try
        {
            var list = await _unitOfWork.OpcionesModificador.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<OpcionModificadorDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "OpcionModificador encontrado";
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
            response.Data = await _unitOfWork.OpcionesModificador.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "OpcionModificador encontrado";
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