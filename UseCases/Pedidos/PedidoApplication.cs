using Interface.Mapping;
using Common;
using Domain.Entities;
using DTO.Pedido;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.Pedidos;

public class PedidoApplication : IPedidoApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppMapper _mapper;
    private readonly PedidoDTOValidator _validationRules;
    private readonly IAppLogger<PedidoApplication> _logger;
    private readonly IFoliadorSucursalService _foliador;

    public PedidoApplication(
        IUnitOfWork unitOfWork,
        IAppMapper mapper,
        PedidoDTOValidator validationRules,
        IAppLogger<PedidoApplication> logger,
        IFoliadorSucursalService foliador)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
        _foliador = foliador;
    }

    #region Metodos sincronos

    public Response<bool> Insert(PedidoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Pedido>(dto);
            response.Data = _unitOfWork.Pedidos.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Pedido creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(PedidoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Pedido>(dto);
            response.Data = _unitOfWork.Pedidos.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Pedido modificado correctamente";
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
            response.Data = _unitOfWork.Pedidos.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Pedido eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<PedidoDTO> Get(Guid id)
    {
        var response = new Response<PedidoDTO>();
        try
        {
            var entity = _unitOfWork.Pedidos.Get(id);
            response.Data = _mapper.Map<PedidoDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Pedido encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<PedidoDTO>> GetAll()
    {
        var response = new Response<IEnumerable<PedidoDTO>>();
        try
        {
            var list = _unitOfWork.Pedidos.GetAll();
            response.Data = _mapper.Map<IEnumerable<PedidoDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Pedido encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<PedidoDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<PedidoDTO>>();
        try
        {
            var list = _unitOfWork.Pedidos.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<PedidoDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "Pedido encontrado";
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
            response.Data = _unitOfWork.Pedidos.Count();
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

    public async Task<Response<bool>> InsertAsync(PedidoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Pedido>(dto);
            response.Data = await _unitOfWork.Pedidos.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Pedido creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<Guid>> InsertConDetallesAsync(CrearPedidoRequestDTO dto)
    {
        var response = new Response<Guid>();
        try
        {
            var entity = _mapper.Map<Pedido>(dto);
            entity.FolioDiario = await _foliador.ObtenerSiguienteFolioAsync(dto.IdSucursal);
            var success = await _unitOfWork.Pedidos.InsertAsync(entity);

            if (success)
            {
                response.Data = entity.Id;
                response.isSuccess = true;
                response.Message = "Pedido con detalles creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(PedidoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Pedido>(dto);
            response.Data = await _unitOfWork.Pedidos.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Pedido modificado correctamente";
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
            response.Data = await _unitOfWork.Pedidos.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Pedido eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<PedidoDTO>> GetAsync(Guid id)
    {
        var response = new Response<PedidoDTO>();
        try
        {
            var entity = await _unitOfWork.Pedidos.GetAsync(id);
            response.Data = _mapper.Map<PedidoDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Pedido encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<PedidoDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<PedidoDTO>>();
        try
        {
            var list = await _unitOfWork.Pedidos.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<PedidoDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Pedido encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<PedidoDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<PedidoDTO>>();
        try
        {
            var list = await _unitOfWork.Pedidos.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<PedidoDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "Pedido encontrado";
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
            response.Data = await _unitOfWork.Pedidos.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Pedido encontrado";
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