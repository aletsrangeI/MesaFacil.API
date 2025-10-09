using AutoMapper;
using Common;
using Domain.Entities;
using DTO.Cuenta;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.Cuentas;

public class CuentaApplication : ICuentaApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly CuentaDTOValidator _validationRules;
    private readonly IAppLogger<CuentaApplication> _logger;

    public CuentaApplication(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        CuentaDTOValidator validationRules,
        IAppLogger<CuentaApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(CuentaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Cuenta>(dto);
            response.Data = _unitOfWork.Cuentas.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Cuenta creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<bool> Update(CuentaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Cuenta>(dto);
            response.Data = _unitOfWork.Cuentas.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Cuenta modificado correctamente";
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
            response.Data = _unitOfWork.Cuentas.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Cuenta eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<CuentaDTO> Get(int id)
    {
        var response = new Response<CuentaDTO>();
        try
        {
            var entity = _unitOfWork.Cuentas.Get(id);
            response.Data = _mapper.Map<CuentaDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Cuenta encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<IEnumerable<CuentaDTO>> GetAll()
    {
        var response = new Response<IEnumerable<CuentaDTO>>();
        try
        {
            var list = _unitOfWork.Cuentas.GetAll();
            response.Data = _mapper.Map<IEnumerable<CuentaDTO>>(list);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public ResponsePagination<IEnumerable<CuentaDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<CuentaDTO>>();
        try
        {
            var list = _unitOfWork.Cuentas.GetAllWithPagination(page, pageSize);

            if (list != null)
            {
                response.Data = _mapper.Map<IEnumerable<CuentaDTO>>(list);
                response.isSuccess = true;
                response.Message = "Cuentas obtenidas";
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
            response.Data = _unitOfWork.Cuentas.Count();
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

    public async Task<Response<bool>> InsertAsync(CuentaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Cuenta>(dto);
            response.Data = await _unitOfWork.Cuentas.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Cuenta creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<bool>> UpdateAsync(CuentaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Cuenta>(dto);
            response.Data = await _unitOfWork.Cuentas.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Cuenta modificado correctamente";
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
            response.Data = await _unitOfWork.Cuentas.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Cuenta eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<CuentaDTO>> GetAsync(int id)
    {
        var response = new Response<CuentaDTO>();
        try
        {
            var entity = await _unitOfWork.Cuentas.GetAsync(id);
            response.Data = _mapper.Map<CuentaDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Cuenta encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<IEnumerable<CuentaDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<CuentaDTO>>();
        try
        {
            var list = await _unitOfWork.Cuentas.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<CuentaDTO>>(list);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<ResponsePagination<IEnumerable<CuentaDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<CuentaDTO>>();
        try
        {
            var list = await _unitOfWork.Cuentas.GetAllWithPaginationAsync(page, pageSize);
            if (list != null)
            {
                response.Data = _mapper.Map<IEnumerable<CuentaDTO>>(list);
                response.isSuccess = true;
                response.Message = "Cuentas obtenidas";
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
            response.Data = await _unitOfWork.Cuentas.CountAsync();
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