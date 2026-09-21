using Interface.Mapping;
using Common;
using Domain.Entities;
using DTO.PedidoAsiento;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.PedidosAsiento;

public class PedidoAsientoApplication : IPedidoAsientoApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppMapper _mapper;
    private readonly PedidoAsientoDTOValidator _validationRules;
    private readonly IAppLogger<PedidoAsientoApplication> _logger;

    public PedidoAsientoApplication(
        IUnitOfWork unitOfWork,
        IAppMapper mapper,
        PedidoAsientoDTOValidator validationRules,
        IAppLogger<PedidoAsientoApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(PedidoAsientoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<PedidoAsiento>(dto);
            response.Data = _unitOfWork.PedidosAsiento.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "PedidoAsiento creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(PedidoAsientoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<PedidoAsiento>(dto);
            response.Data = _unitOfWork.PedidosAsiento.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "PedidoAsiento modificado correctamente";
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
            response.Data = _unitOfWork.PedidosAsiento.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "PedidoAsiento eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<PedidoAsientoDTO> Get(Guid id)
    {
        var response = new Response<PedidoAsientoDTO>();
        try
        {
            var entity = _unitOfWork.PedidosAsiento.Get(id);
            response.Data = _mapper.Map<PedidoAsientoDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "PedidoAsiento encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<PedidoAsientoDTO>> GetAll()
    {
        var response = new Response<IEnumerable<PedidoAsientoDTO>>();
        try
        {
            var list = _unitOfWork.PedidosAsiento.GetAll();
            response.Data = _mapper.Map<IEnumerable<PedidoAsientoDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "PedidoAsiento encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<PedidoAsientoDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<PedidoAsientoDTO>>();
        try
        {
            var list = _unitOfWork.PedidosAsiento.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<PedidoAsientoDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "PedidoAsiento encontrado";
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
            response.Data = _unitOfWork.PedidosAsiento.Count();
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

    public async Task<Response<bool>> InsertAsync(PedidoAsientoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<PedidoAsiento>(dto);
            response.Data = await _unitOfWork.PedidosAsiento.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "PedidoAsiento creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(PedidoAsientoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<PedidoAsiento>(dto);
            response.Data = await _unitOfWork.PedidosAsiento.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "PedidoAsiento modificado correctamente";
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
            response.Data = await _unitOfWork.PedidosAsiento.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "PedidoAsiento eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<PedidoAsientoDTO>> GetAsync(Guid id)
    {
        var response = new Response<PedidoAsientoDTO>();
        try
        {
            var entity = await _unitOfWork.PedidosAsiento.GetAsync(id);
            response.Data = _mapper.Map<PedidoAsientoDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "PedidoAsiento encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<PedidoAsientoDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<PedidoAsientoDTO>>();
        try
        {
            var list = await _unitOfWork.PedidosAsiento.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<PedidoAsientoDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "PedidoAsiento encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<PedidoAsientoDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<PedidoAsientoDTO>>();
        try
        {
            var list = await _unitOfWork.PedidosAsiento.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<PedidoAsientoDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "PedidoAsiento encontrado";
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
            response.Data = await _unitOfWork.PedidosAsiento.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "PedidoAsiento encontrado";
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