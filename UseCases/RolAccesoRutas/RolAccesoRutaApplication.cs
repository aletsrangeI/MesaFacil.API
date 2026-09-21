using Interface.Mapping;
using Common;
using Domain.Entities;
using DTO.RolAccesoRuta;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.RolAccesoRutas;

public class RolAccesoRutaApplication : IRolAccesoRutaApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppMapper _mapper;
    private readonly RolAccesoRutaDTOValidator _validationRules;
    private readonly IAppLogger<RolAccesoRutaApplication> _logger;

    public RolAccesoRutaApplication(
        IUnitOfWork unitOfWork,
        IAppMapper mapper,
        RolAccesoRutaDTOValidator validationRules,
        IAppLogger<RolAccesoRutaApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(RolAccesoRutaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<RolAccesoRuta>(dto);
            response.Data = _unitOfWork.RolAccesoRutas.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "RolAccesoRuta creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(RolAccesoRutaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<RolAccesoRuta>(dto);
            response.Data = _unitOfWork.RolAccesoRutas.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "RolAccesoRuta modificado correctamente";
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
            response.Data = _unitOfWork.RolAccesoRutas.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "RolAccesoRuta eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<RolAccesoRutaDTO> Get(int id)
    {
        var response = new Response<RolAccesoRutaDTO>();
        try
        {
            var entity = _unitOfWork.RolAccesoRutas.Get(id);
            response.Data = _mapper.Map<RolAccesoRutaDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "RolAccesoRuta encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<RolAccesoRutaDTO>> GetAll()
    {
        var response = new Response<IEnumerable<RolAccesoRutaDTO>>();
        try
        {
            var list = _unitOfWork.RolAccesoRutas.GetAll();
            response.Data = _mapper.Map<IEnumerable<RolAccesoRutaDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "RolAccesoRuta encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<RolAccesoRutaDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<RolAccesoRutaDTO>>();
        try
        {
            var list = _unitOfWork.RolAccesoRutas.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<RolAccesoRutaDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "RolAccesoRuta encontrado";
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
            response.Data = _unitOfWork.RolAccesoRutas.Count();
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

    public async Task<Response<bool>> InsertAsync(RolAccesoRutaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<RolAccesoRuta>(dto);
            response.Data = await _unitOfWork.RolAccesoRutas.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "RolAccesoRuta creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(RolAccesoRutaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<RolAccesoRuta>(dto);
            response.Data = await _unitOfWork.RolAccesoRutas.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "RolAccesoRuta modificado correctamente";
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
            response.Data = await _unitOfWork.RolAccesoRutas.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "RolAccesoRuta eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<RolAccesoRutaDTO>> GetAsync(int id)
    {
        var response = new Response<RolAccesoRutaDTO>();
        try
        {
            var entity = await _unitOfWork.RolAccesoRutas.GetAsync(id);
            response.Data = _mapper.Map<RolAccesoRutaDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "RolAccesoRuta encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<RolAccesoRutaDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<RolAccesoRutaDTO>>();
        try
        {
            var list = await _unitOfWork.RolAccesoRutas.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<RolAccesoRutaDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "RolAccesoRuta encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<RolAccesoRutaDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<RolAccesoRutaDTO>>();
        try
        {
            var list = await _unitOfWork.RolAccesoRutas.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<RolAccesoRutaDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "RolAccesoRuta encontrado";
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
            response.Data = await _unitOfWork.RolAccesoRutas.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "RolAccesoRuta encontrado";
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