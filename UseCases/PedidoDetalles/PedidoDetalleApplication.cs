using Interface.Mapping;
using Common;
using Domain.Entities;
using DTO.PedidoDetalle;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.PedidoDetalles;

public class PedidoDetalleApplication : IPedidoDetalleApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppMapper _mapper;
    private readonly PedidoDetalleDTOValidator _validationRules;
    private readonly IAppLogger<PedidoDetalleApplication> _logger;

    public PedidoDetalleApplication(
        IUnitOfWork unitOfWork,
        IAppMapper mapper,
        PedidoDetalleDTOValidator validationRules,
        IAppLogger<PedidoDetalleApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(PedidoDetalleDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<PedidoDetalle>(dto);
            response.Data = _unitOfWork.PedidoDetalles.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "PedidoDetalle creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(PedidoDetalleDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<PedidoDetalle>(dto);
            response.Data = _unitOfWork.PedidoDetalles.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "PedidoDetalle modificado correctamente";
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
            response.Data = _unitOfWork.PedidoDetalles.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "PedidoDetalle eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<PedidoDetalleDTO> Get(Guid id)
    {
        var response = new Response<PedidoDetalleDTO>();
        try
        {
            var entity = _unitOfWork.PedidoDetalles.Get(id);
            response.Data = _mapper.Map<PedidoDetalleDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "PedidoDetalle encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<PedidoDetalleDTO>> GetAll()
    {
        var response = new Response<IEnumerable<PedidoDetalleDTO>>();
        try
        {
            var list = _unitOfWork.PedidoDetalles.GetAll();
            response.Data = _mapper.Map<IEnumerable<PedidoDetalleDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "PedidoDetalle encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<PedidoDetalleDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<PedidoDetalleDTO>>();
        try
        {
            var list = _unitOfWork.PedidoDetalles.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<PedidoDetalleDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "PedidoDetalle encontrado";
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
            response.Data = _unitOfWork.PedidoDetalles.Count();
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

    public async Task<Response<bool>> InsertAsync(PedidoDetalleDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<PedidoDetalle>(dto);
            response.Data = await _unitOfWork.PedidoDetalles.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "PedidoDetalle creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(PedidoDetalleDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<PedidoDetalle>(dto);
            response.Data = await _unitOfWork.PedidoDetalles.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "PedidoDetalle modificado correctamente";
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
            response.Data = await _unitOfWork.PedidoDetalles.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "PedidoDetalle eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<PedidoDetalleDTO>> GetAsync(Guid id)
    {
        var response = new Response<PedidoDetalleDTO>();
        try
        {
            var entity = await _unitOfWork.PedidoDetalles.GetAsync(id);
            response.Data = _mapper.Map<PedidoDetalleDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "PedidoDetalle encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<PedidoDetalleDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<PedidoDetalleDTO>>();
        try
        {
            var list = await _unitOfWork.PedidoDetalles.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<PedidoDetalleDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "PedidoDetalle encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<PedidoDetalleDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<PedidoDetalleDTO>>();
        try
        {
            var list = await _unitOfWork.PedidoDetalles.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<PedidoDetalleDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "PedidoDetalle encontrado";
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
            response.Data = await _unitOfWork.PedidoDetalles.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "PedidoDetalle encontrado";
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