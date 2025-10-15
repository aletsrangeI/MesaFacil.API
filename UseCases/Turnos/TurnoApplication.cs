using AutoMapper;
using Common;
using Domain.Entities;
using DTO.Turno;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.Turnos;

public class TurnoApplication : ITurnoApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly TurnoDTOValidator _validationRules;
    private readonly IAppLogger<TurnoApplication> _logger;

    public TurnoApplication(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        TurnoDTOValidator validationRules,
        IAppLogger<TurnoApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(TurnoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Turno>(dto);
            response.Data = _unitOfWork.Turnos.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Turno creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(TurnoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Turno>(dto);
            response.Data = _unitOfWork.Turnos.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Turno modificado correctamente";
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
            response.Data = _unitOfWork.Turnos.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Turno eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<TurnoDTO> Get(int id)
    {
        var response = new Response<TurnoDTO>();
        try
        {
            var entity = _unitOfWork.Turnos.Get(id);
            response.Data = _mapper.Map<TurnoDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Turno encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<TurnoDTO>> GetAll()
    {
        var response = new Response<IEnumerable<TurnoDTO>>();
        try
        {
            var list = _unitOfWork.Turnos.GetAll();
            response.Data = _mapper.Map<IEnumerable<TurnoDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Turno encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<TurnoDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<TurnoDTO>>();
        try
        {
            var list = _unitOfWork.Turnos.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<TurnoDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "Turno encontrado";
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
            response.Data = _unitOfWork.Turnos.Count();
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

    public async Task<Response<bool>> InsertAsync(TurnoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Turno>(dto);
            response.Data = await _unitOfWork.Turnos.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Turno creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(TurnoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Turno>(dto);
            response.Data = await _unitOfWork.Turnos.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Turno modificado correctamente";
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
            response.Data = await _unitOfWork.Turnos.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Turno eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<TurnoDTO>> GetAsync(int id)
    {
        var response = new Response<TurnoDTO>();
        try
        {
            var entity = await _unitOfWork.Turnos.GetAsync(id);
            response.Data = _mapper.Map<TurnoDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Turno encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<TurnoDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<TurnoDTO>>();
        try
        {
            var list = await _unitOfWork.Turnos.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<TurnoDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Turno encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<TurnoDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<TurnoDTO>>();
        try
        {
            var list = await _unitOfWork.Turnos.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<TurnoDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "Turno encontrado";
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
            response.Data = await _unitOfWork.Turnos.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Turno encontrado";
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