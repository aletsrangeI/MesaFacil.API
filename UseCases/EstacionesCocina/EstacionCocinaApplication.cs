using Interface.Mapping;
using Common;
using Domain.Entities;
using DTO.EstacionCocina;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.EstacionesCocina;

public class EstacionCocinaApplication : IEstacionCocinaApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppMapper _mapper;
    private readonly EstacionCocinaDTOValidator _validationRules;
    private readonly IAppLogger<EstacionCocinaApplication> _logger;

    public EstacionCocinaApplication(
        IUnitOfWork unitOfWork,
        IAppMapper mapper,
        EstacionCocinaDTOValidator validationRules,
        IAppLogger<EstacionCocinaApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(EstacionCocinaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<EstacionCocina>(dto);
            response.Data = _unitOfWork.EstacionesCocina.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "EstacionCocina creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(EstacionCocinaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<EstacionCocina>(dto);
            response.Data = _unitOfWork.EstacionesCocina.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "EstacionCocina modificado correctamente";
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
            response.Data = _unitOfWork.EstacionesCocina.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "EstacionCocina eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<EstacionCocinaDTO> Get(int id)
    {
        var response = new Response<EstacionCocinaDTO>();
        try
        {
            var entity = _unitOfWork.EstacionesCocina.Get(id);
            response.Data = _mapper.Map<EstacionCocinaDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "EstacionCocina encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<EstacionCocinaDTO>> GetAll()
    {
        var response = new Response<IEnumerable<EstacionCocinaDTO>>();
        try
        {
            var list = _unitOfWork.EstacionesCocina.GetAll();
            response.Data = _mapper.Map<IEnumerable<EstacionCocinaDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "EstacionCocina encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<EstacionCocinaDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<EstacionCocinaDTO>>();
        try
        {
            var list = _unitOfWork.EstacionesCocina.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<EstacionCocinaDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "EstacionCocina encontrado";
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
            response.Data = _unitOfWork.EstacionesCocina.Count();
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

    public async Task<Response<bool>> InsertAsync(EstacionCocinaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<EstacionCocina>(dto);
            response.Data = await _unitOfWork.EstacionesCocina.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "EstacionCocina creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(EstacionCocinaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<EstacionCocina>(dto);
            response.Data = await _unitOfWork.EstacionesCocina.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "EstacionCocina modificado correctamente";
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
            response.Data = await _unitOfWork.EstacionesCocina.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "EstacionCocina eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<EstacionCocinaDTO>> GetAsync(int id)
    {
        var response = new Response<EstacionCocinaDTO>();
        try
        {
            var entity = await _unitOfWork.EstacionesCocina.GetAsync(id);
            response.Data = _mapper.Map<EstacionCocinaDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "EstacionCocina encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<EstacionCocinaDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<EstacionCocinaDTO>>();
        try
        {
            var list = await _unitOfWork.EstacionesCocina.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<EstacionCocinaDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "EstacionCocina encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<EstacionCocinaDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<EstacionCocinaDTO>>();
        try
        {
            var list = await _unitOfWork.EstacionesCocina.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<EstacionCocinaDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "EstacionCocina encontrado";
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
            response.Data = await _unitOfWork.EstacionesCocina.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "EstacionCocina encontrado";
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