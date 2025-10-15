using AutoMapper;
using Common;
using Domain.Entities;
using DTO.MovimientoCaja;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.MovimientosCaja;

public class MovimientoCajaApplication : IMovimientoCajaApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly MovimientoCajaDTOValidator _validationRules;
    private readonly IAppLogger<MovimientoCajaApplication> _logger;

    public MovimientoCajaApplication(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        MovimientoCajaDTOValidator validationRules,
        IAppLogger<MovimientoCajaApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(MovimientoCajaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<MovimientoCaja>(dto);
            response.Data = _unitOfWork.MovimientosCaja.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "MovimientoCaja creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(MovimientoCajaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<MovimientoCaja>(dto);
            response.Data = _unitOfWork.MovimientosCaja.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "MovimientoCaja modificado correctamente";
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
            response.Data = _unitOfWork.MovimientosCaja.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "MovimientoCaja eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<MovimientoCajaDTO> Get(int id)
    {
        var response = new Response<MovimientoCajaDTO>();
        try
        {
            var entity = _unitOfWork.MovimientosCaja.Get(id);
            response.Data = _mapper.Map<MovimientoCajaDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "MovimientoCaja encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<MovimientoCajaDTO>> GetAll()
    {
        var response = new Response<IEnumerable<MovimientoCajaDTO>>();
        try
        {
            var list = _unitOfWork.MovimientosCaja.GetAll();
            response.Data = _mapper.Map<IEnumerable<MovimientoCajaDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "MovimientoCaja encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<MovimientoCajaDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<MovimientoCajaDTO>>();
        try
        {
            var list = _unitOfWork.MovimientosCaja.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<MovimientoCajaDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "MovimientoCaja encontrado";
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
            response.Data = _unitOfWork.MovimientosCaja.Count();
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

    public async Task<Response<bool>> InsertAsync(MovimientoCajaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<MovimientoCaja>(dto);
            response.Data = await _unitOfWork.MovimientosCaja.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "MovimientoCaja creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(MovimientoCajaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<MovimientoCaja>(dto);
            response.Data = await _unitOfWork.MovimientosCaja.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "MovimientoCaja modificado correctamente";
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
            response.Data = await _unitOfWork.MovimientosCaja.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "MovimientoCaja eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<MovimientoCajaDTO>> GetAsync(int id)
    {
        var response = new Response<MovimientoCajaDTO>();
        try
        {
            var entity = await _unitOfWork.MovimientosCaja.GetAsync(id);
            response.Data = _mapper.Map<MovimientoCajaDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "MovimientoCaja encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<MovimientoCajaDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<MovimientoCajaDTO>>();
        try
        {
            var list = await _unitOfWork.MovimientosCaja.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<MovimientoCajaDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "MovimientoCaja encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<MovimientoCajaDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<MovimientoCajaDTO>>();
        try
        {
            var list = await _unitOfWork.MovimientosCaja.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<MovimientoCajaDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "MovimientoCaja encontrado";
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
            response.Data = await _unitOfWork.MovimientosCaja.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "MovimientoCaja encontrado";
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