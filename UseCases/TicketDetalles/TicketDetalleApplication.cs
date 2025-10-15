using AutoMapper;
using Common;
using Domain.Entities;
using DTO.TicketDetalle;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.TicketDetalles;

public class TicketDetalleApplication : ITicketDetalleApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly TicketDetalleDTOValidator _validationRules;
    private readonly IAppLogger<TicketDetalleApplication> _logger;

    public TicketDetalleApplication(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        TicketDetalleDTOValidator validationRules,
        IAppLogger<TicketDetalleApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(TicketDetalleDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<TicketDetalle>(dto);
            response.Data = _unitOfWork.TicketDetalles.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "TicketDetalle creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(TicketDetalleDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<TicketDetalle>(dto);
            response.Data = _unitOfWork.TicketDetalles.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "TicketDetalle modificado correctamente";
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
            response.Data = _unitOfWork.TicketDetalles.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "TicketDetalle eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<TicketDetalleDTO> Get(int id)
    {
        var response = new Response<TicketDetalleDTO>();
        try
        {
            var entity = _unitOfWork.TicketDetalles.Get(id);
            response.Data = _mapper.Map<TicketDetalleDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "TicketDetalle encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<TicketDetalleDTO>> GetAll()
    {
        var response = new Response<IEnumerable<TicketDetalleDTO>>();
        try
        {
            var list = _unitOfWork.TicketDetalles.GetAll();
            response.Data = _mapper.Map<IEnumerable<TicketDetalleDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "TicketDetalle encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<TicketDetalleDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<TicketDetalleDTO>>();
        try
        {
            var list = _unitOfWork.TicketDetalles.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<TicketDetalleDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "TicketDetalle encontrado";
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
            response.Data = _unitOfWork.TicketDetalles.Count();
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

    public async Task<Response<bool>> InsertAsync(TicketDetalleDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<TicketDetalle>(dto);
            response.Data = await _unitOfWork.TicketDetalles.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "TicketDetalle creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(TicketDetalleDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<TicketDetalle>(dto);
            response.Data = await _unitOfWork.TicketDetalles.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "TicketDetalle modificado correctamente";
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
            response.Data = await _unitOfWork.TicketDetalles.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "TicketDetalle eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<TicketDetalleDTO>> GetAsync(int id)
    {
        var response = new Response<TicketDetalleDTO>();
        try
        {
            var entity = await _unitOfWork.TicketDetalles.GetAsync(id);
            response.Data = _mapper.Map<TicketDetalleDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "TicketDetalle encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<TicketDetalleDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<TicketDetalleDTO>>();
        try
        {
            var list = await _unitOfWork.TicketDetalles.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<TicketDetalleDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "TicketDetalle encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<TicketDetalleDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<TicketDetalleDTO>>();
        try
        {
            var list = await _unitOfWork.TicketDetalles.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<TicketDetalleDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "TicketDetalle encontrado";
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
            response.Data = await _unitOfWork.TicketDetalles.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "TicketDetalle encontrado";
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