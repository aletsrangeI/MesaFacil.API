using AutoMapper;
using Common;
using Domain.Entities;
using DTO.EventoPedido;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.EventosPedido;

public class EventoPedidoApplication : IEventoPedidoApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly EventoPedidoDTOValidator _validationRules;
    private readonly IAppLogger<EventoPedidoApplication> _logger;

    public EventoPedidoApplication(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        EventoPedidoDTOValidator validationRules,
        IAppLogger<EventoPedidoApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(EventoPedidoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<EventoPedido>(dto);
            response.Data = _unitOfWork.EventosPedido.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "EventoPedido creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(EventoPedidoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<EventoPedido>(dto);
            response.Data = _unitOfWork.EventosPedido.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "EventoPedido modificado correctamente";
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
            response.Data = _unitOfWork.EventosPedido.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "EventoPedido eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<EventoPedidoDTO> Get(int id)
    {
        var response = new Response<EventoPedidoDTO>();
        try
        {
            var entity = _unitOfWork.EventosPedido.Get(id);
            response.Data = _mapper.Map<EventoPedidoDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "EventoPedido encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<EventoPedidoDTO>> GetAll()
    {
        var response = new Response<IEnumerable<EventoPedidoDTO>>();
        try
        {
            var list = _unitOfWork.EventosPedido.GetAll();
            response.Data = _mapper.Map<IEnumerable<EventoPedidoDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "EventoPedido encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<EventoPedidoDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<EventoPedidoDTO>>();
        try
        {
            var list = _unitOfWork.EventosPedido.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<EventoPedidoDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "EventoPedido encontrado";
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
            response.Data = _unitOfWork.EventosPedido.Count();
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

    public async Task<Response<bool>> InsertAsync(EventoPedidoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<EventoPedido>(dto);
            response.Data = await _unitOfWork.EventosPedido.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "EventoPedido creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(EventoPedidoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<EventoPedido>(dto);
            response.Data = await _unitOfWork.EventosPedido.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "EventoPedido modificado correctamente";
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
            response.Data = await _unitOfWork.EventosPedido.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "EventoPedido eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<EventoPedidoDTO>> GetAsync(int id)
    {
        var response = new Response<EventoPedidoDTO>();
        try
        {
            var entity = await _unitOfWork.EventosPedido.GetAsync(id);
            response.Data = _mapper.Map<EventoPedidoDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "EventoPedido encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<EventoPedidoDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<EventoPedidoDTO>>();
        try
        {
            var list = await _unitOfWork.EventosPedido.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<EventoPedidoDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "EventoPedido encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<EventoPedidoDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<EventoPedidoDTO>>();
        try
        {
            var list = await _unitOfWork.EventosPedido.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<EventoPedidoDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "EventoPedido encontrado";
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
            response.Data = await _unitOfWork.EventosPedido.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "EventoPedido encontrado";
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