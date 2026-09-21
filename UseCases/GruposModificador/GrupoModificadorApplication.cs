using Interface.Mapping;
using Common;
using Domain.Entities;
using DTO.GrupoModificador;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.GruposModificador;

public class GrupoModificadorApplication : IGrupoModificadorApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppMapper _mapper;
    private readonly GrupoModificadorDTOValidator _validationRules;
    private readonly IAppLogger<GrupoModificadorApplication> _logger;

    public GrupoModificadorApplication(
        IUnitOfWork unitOfWork,
        IAppMapper mapper,
        GrupoModificadorDTOValidator validationRules,
        IAppLogger<GrupoModificadorApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(GrupoModificadorDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<GrupoModificador>(dto);
            response.Data = _unitOfWork.GruposModificador.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "GrupoModificador creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(GrupoModificadorDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<GrupoModificador>(dto);
            response.Data = _unitOfWork.GruposModificador.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "GrupoModificador modificado correctamente";
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
            response.Data = _unitOfWork.GruposModificador.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "GrupoModificador eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<GrupoModificadorDTO> Get(int id)
    {
        var response = new Response<GrupoModificadorDTO>();
        try
        {
            var entity = _unitOfWork.GruposModificador.Get(id);
            response.Data = _mapper.Map<GrupoModificadorDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "GrupoModificador encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<GrupoModificadorDTO>> GetAll()
    {
        var response = new Response<IEnumerable<GrupoModificadorDTO>>();
        try
        {
            var list = _unitOfWork.GruposModificador.GetAll();
            response.Data = _mapper.Map<IEnumerable<GrupoModificadorDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "GrupoModificador encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<GrupoModificadorDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<GrupoModificadorDTO>>();
        try
        {
            var list = _unitOfWork.GruposModificador.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<GrupoModificadorDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "GrupoModificador encontrado";
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
            response.Data = _unitOfWork.GruposModificador.Count();
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

    public async Task<Response<bool>> InsertAsync(GrupoModificadorDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<GrupoModificador>(dto);
            response.Data = await _unitOfWork.GruposModificador.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "GrupoModificador creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(GrupoModificadorDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<GrupoModificador>(dto);
            response.Data = await _unitOfWork.GruposModificador.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "GrupoModificador modificado correctamente";
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
            response.Data = await _unitOfWork.GruposModificador.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "GrupoModificador eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<GrupoModificadorDTO>> GetAsync(int id)
    {
        var response = new Response<GrupoModificadorDTO>();
        try
        {
            var entity = await _unitOfWork.GruposModificador.GetAsync(id);
            response.Data = _mapper.Map<GrupoModificadorDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "GrupoModificador encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<GrupoModificadorDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<GrupoModificadorDTO>>();
        try
        {
            var list = await _unitOfWork.GruposModificador.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<GrupoModificadorDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "GrupoModificador encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<GrupoModificadorDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<GrupoModificadorDTO>>();
        try
        {
            var list = await _unitOfWork.GruposModificador.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<GrupoModificadorDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "GrupoModificador encontrado";
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
            response.Data = await _unitOfWork.GruposModificador.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "GrupoModificador encontrado";
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