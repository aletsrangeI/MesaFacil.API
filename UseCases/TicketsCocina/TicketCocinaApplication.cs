using AutoMapper;
using Common;
using Domain.Entities;
using DTO.TicketCocina;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.TicketsCocina;

public class TicketCocinaApplication : ITicketCocinaApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly TicketCocinaDTOValidator _validationRules;
    private readonly IAppLogger<TicketCocinaApplication> _logger;

    public TicketCocinaApplication(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        TicketCocinaDTOValidator validationRules,
        IAppLogger<TicketCocinaApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<Guid> Insert(TicketCocinaDTO dto)
    {
        var response = new Response<Guid>();
        try
        {
            var entity = _mapper.Map<TicketCocina>(dto);
            var success = _unitOfWork.TicketsCocina.Insert(entity);

            if (success)
            {
                response.Data = entity.Id;
                response.isSuccess = true;
                response.Message = "TicketCocina creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(TicketCocinaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<TicketCocina>(dto);
            response.Data = _unitOfWork.TicketsCocina.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "TicketCocina modificado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Delete(Guid id)
    {
        var response = new Response<bool>();
        try
        {
            response.Data = _unitOfWork.TicketsCocina.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "TicketCocina eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<TicketCocinaDTO> Get(Guid id)
    {
        var response = new Response<TicketCocinaDTO>();
        try
        {
            var entity = _unitOfWork.TicketsCocina.Get(id);
            response.Data = _mapper.Map<TicketCocinaDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "TicketCocina encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<TicketCocinaDTO>> GetAll()
    {
        var response = new Response<IEnumerable<TicketCocinaDTO>>();
        try
        {
            var list = _unitOfWork.TicketsCocina.GetAll();
            response.Data = _mapper.Map<IEnumerable<TicketCocinaDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "TicketCocina encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<TicketCocinaDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<TicketCocinaDTO>>();
        try
        {
            var list = _unitOfWork.TicketsCocina.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<TicketCocinaDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "TicketCocina encontrado";
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
            response.Data = _unitOfWork.TicketsCocina.Count();
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

    public async Task<Response<Guid>> InsertAsync(TicketCocinaDTO dto)
    {
        var response = new Response<Guid>();
        try
        {
            var entity = _mapper.Map<TicketCocina>(dto);
            var success = await _unitOfWork.TicketsCocina.InsertAsync(entity);

            if (success)
            {
                response.Data = entity.Id;
                response.isSuccess = true;
                response.Message = "TicketCocina creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(TicketCocinaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<TicketCocina>(dto);
            response.Data = await _unitOfWork.TicketsCocina.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "TicketCocina modificado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> DeleteAsync(Guid id)
    {
        var response = new Response<bool>();
        try
        {
            response.Data = await _unitOfWork.TicketsCocina.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "TicketCocina eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<TicketCocinaDTO>> GetAsync(Guid id)
    {
        var response = new Response<TicketCocinaDTO>();
        try
        {
            var entity = await _unitOfWork.TicketsCocina.GetAsync(id);
            response.Data = _mapper.Map<TicketCocinaDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "TicketCocina encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<TicketCocinaDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<TicketCocinaDTO>>();
        try
        {
            var list = await _unitOfWork.TicketsCocina.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<TicketCocinaDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "TicketCocina encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<TicketCocinaDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<TicketCocinaDTO>>();
        try
        {
            var list = await _unitOfWork.TicketsCocina.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<TicketCocinaDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "TicketCocina encontrado";
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
            response.Data = await _unitOfWork.TicketsCocina.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "TicketCocina encontrado";
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