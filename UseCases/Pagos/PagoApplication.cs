using AutoMapper;
using Common;
using Domain.Entities;
using DTO.Pago;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.Pagos;

public class PagoApplication : IPagoApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly PagoDTOValidator _validationRules;
    private readonly IAppLogger<PagoApplication> _logger;

    public PagoApplication(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        PagoDTOValidator validationRules,
        IAppLogger<PagoApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(PagoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Pago>(dto);
            response.Data = _unitOfWork.Pagos.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Pago creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(PagoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Pago>(dto);
            response.Data = _unitOfWork.Pagos.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Pago modificado correctamente";
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
            response.Data = _unitOfWork.Pagos.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Pago eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<PagoDTO> Get(int id)
    {
        var response = new Response<PagoDTO>();
        try
        {
            var entity = _unitOfWork.Pagos.Get(id);
            response.Data = _mapper.Map<PagoDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Pago encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<PagoDTO>> GetAll()
    {
        var response = new Response<IEnumerable<PagoDTO>>();
        try
        {
            var list = _unitOfWork.Pagos.GetAll();
            response.Data = _mapper.Map<IEnumerable<PagoDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Pago encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<PagoDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<PagoDTO>>();
        try
        {
            var list = _unitOfWork.Pagos.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<PagoDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "Pago encontrado";
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
            response.Data = _unitOfWork.Pagos.Count();
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

    public async Task<Response<bool>> InsertAsync(PagoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Pago>(dto);
            response.Data = await _unitOfWork.Pagos.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Pago creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(PagoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Pago>(dto);
            response.Data = await _unitOfWork.Pagos.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Pago modificado correctamente";
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
            response.Data = await _unitOfWork.Pagos.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Pago eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<PagoDTO>> GetAsync(int id)
    {
        var response = new Response<PagoDTO>();
        try
        {
            var entity = await _unitOfWork.Pagos.GetAsync(id);
            response.Data = _mapper.Map<PagoDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Pago encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<PagoDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<PagoDTO>>();
        try
        {
            var list = await _unitOfWork.Pagos.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<PagoDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Pago encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<PagoDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<PagoDTO>>();
        try
        {
            var list = await _unitOfWork.Pagos.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<PagoDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "Pago encontrado";
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
            response.Data = await _unitOfWork.Pagos.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Pago encontrado";
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