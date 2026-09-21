using Interface.Mapping;
using Common;
using Domain.Entities;
using DTO.DetalleCuenta;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.DetalleCuentas;

public class DetalleCuentaApplication : IDetalleCuentaApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppMapper _mapper;
    private readonly DetalleCuentaDTOValidator _validationRules;
    private readonly IAppLogger<DetalleCuentaApplication> _logger;

    public DetalleCuentaApplication(
        IUnitOfWork unitOfWork,
        IAppMapper mapper,
        DetalleCuentaDTOValidator validationRules,
        IAppLogger<DetalleCuentaApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(DetalleCuentaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<DetalleCuenta>(dto);
            response.Data = _unitOfWork.DetalleCuentas.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "DetalleCuenta creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(DetalleCuentaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<DetalleCuenta>(dto);
            response.Data = _unitOfWork.DetalleCuentas.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "DetalleCuenta modificado correctamente";
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
            response.Data = _unitOfWork.DetalleCuentas.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "DetalleCuenta eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<DetalleCuentaDTO> Get(int id)
    {
        var response = new Response<DetalleCuentaDTO>();
        try
        {
            var entity = _unitOfWork.DetalleCuentas.Get(id);
            response.Data = _mapper.Map<DetalleCuentaDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "DetalleCuenta encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<DetalleCuentaDTO>> GetAll()
    {
        var response = new Response<IEnumerable<DetalleCuentaDTO>>();
        try
        {
            var list = _unitOfWork.DetalleCuentas.GetAll();
            response.Data = _mapper.Map<IEnumerable<DetalleCuentaDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "DetalleCuenta encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<DetalleCuentaDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<DetalleCuentaDTO>>();
        try
        {
            var list = _unitOfWork.DetalleCuentas.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<DetalleCuentaDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "DetalleCuenta encontrado";
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
            response.Data = _unitOfWork.DetalleCuentas.Count();
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

    public async Task<Response<bool>> InsertAsync(DetalleCuentaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<DetalleCuenta>(dto);
            response.Data = await _unitOfWork.DetalleCuentas.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "DetalleCuenta creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(DetalleCuentaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<DetalleCuenta>(dto);
            response.Data = await _unitOfWork.DetalleCuentas.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "DetalleCuenta modificado correctamente";
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
            response.Data = await _unitOfWork.DetalleCuentas.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "DetalleCuenta eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<DetalleCuentaDTO>> GetAsync(int id)
    {
        var response = new Response<DetalleCuentaDTO>();
        try
        {
            var entity = await _unitOfWork.DetalleCuentas.GetAsync(id);
            response.Data = _mapper.Map<DetalleCuentaDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "DetalleCuenta encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<DetalleCuentaDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<DetalleCuentaDTO>>();
        try
        {
            var list = await _unitOfWork.DetalleCuentas.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<DetalleCuentaDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "DetalleCuenta encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<DetalleCuentaDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<DetalleCuentaDTO>>();
        try
        {
            var list = await _unitOfWork.DetalleCuentas.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<DetalleCuentaDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "DetalleCuenta encontrado";
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
            response.Data = await _unitOfWork.DetalleCuentas.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "DetalleCuenta encontrado";
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