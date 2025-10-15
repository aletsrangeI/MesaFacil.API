using AutoMapper;
using Common;
using Domain.Entities;
using DTO.Sucursal;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.Sucursales;

public class SucursalApplication : ISucursalApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly SucursalDTOValidator _validationRules;
    private readonly IAppLogger<SucursalApplication> _logger;

    public SucursalApplication(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        SucursalDTOValidator validationRules,
        IAppLogger<SucursalApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(SucursalDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Sucursal>(dto);
            response.Data = _unitOfWork.Sucursales.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Sucursal creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(SucursalDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Sucursal>(dto);
            response.Data = _unitOfWork.Sucursales.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Sucursal modificado correctamente";
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
            response.Data = _unitOfWork.Sucursales.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Sucursal eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<SucursalDTO> Get(int id)
    {
        var response = new Response<SucursalDTO>();
        try
        {
            var entity = _unitOfWork.Sucursales.Get(id);
            response.Data = _mapper.Map<SucursalDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Sucursal encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<SucursalDTO>> GetAll()
    {
        var response = new Response<IEnumerable<SucursalDTO>>();
        try
        {
            var list = _unitOfWork.Sucursales.GetAll();
            response.Data = _mapper.Map<IEnumerable<SucursalDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Sucursal encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<SucursalDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<SucursalDTO>>();
        try
        {
            var list = _unitOfWork.Sucursales.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<SucursalDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "Sucursal encontrado";
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
            response.Data = _unitOfWork.Sucursales.Count();
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

    public async Task<Response<bool>> InsertAsync(SucursalDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Sucursal>(dto);
            response.Data = await _unitOfWork.Sucursales.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Sucursal creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(SucursalDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Sucursal>(dto);
            response.Data = await _unitOfWork.Sucursales.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Sucursal modificado correctamente";
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
            response.Data = await _unitOfWork.Sucursales.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Sucursal eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<SucursalDTO>> GetAsync(int id)
    {
        var response = new Response<SucursalDTO>();
        try
        {
            var entity = await _unitOfWork.Sucursales.GetAsync(id);
            response.Data = _mapper.Map<SucursalDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Sucursal encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<SucursalDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<SucursalDTO>>();
        try
        {
            var list = await _unitOfWork.Sucursales.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<SucursalDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Sucursal encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<SucursalDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<SucursalDTO>>();
        try
        {
            var list = await _unitOfWork.Sucursales.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<SucursalDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "Sucursal encontrado";
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
            response.Data = await _unitOfWork.Sucursales.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Sucursal encontrado";
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