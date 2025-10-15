using AutoMapper;
using Common;
using Domain.Entities;
using DTO.AccesoRuta;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.AccesoRutas;

public class AccesoRutaApplication : IAccesoRutaApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly AccesoRutaDTOValidator _validationRules;
    private readonly IAppLogger<AccesoRutaApplication> _logger;

    public AccesoRutaApplication(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        AccesoRutaDTOValidator validationRules,
        IAppLogger<AccesoRutaApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(AccesoRutaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<AccesoRuta>(dto);
            response.Data = _unitOfWork.AccesoRutas.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "AccesoRuta creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(AccesoRutaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<AccesoRuta>(dto);
            response.Data = _unitOfWork.AccesoRutas.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "AccesoRuta modificado correctamente";
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
            response.Data = _unitOfWork.AccesoRutas.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "AccesoRuta eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<AccesoRutaDTO> Get(int id)
    {
        var response = new Response<AccesoRutaDTO>();
        try
        {
            var entity = _unitOfWork.AccesoRutas.Get(id);
            response.Data = _mapper.Map<AccesoRutaDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "AccesoRuta encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<AccesoRutaDTO>> GetAll()
    {
        var response = new Response<IEnumerable<AccesoRutaDTO>>();
        try
        {
            var list = _unitOfWork.AccesoRutas.GetAll();
            response.Data = _mapper.Map<IEnumerable<AccesoRutaDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "AccesoRuta encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<AccesoRutaDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<AccesoRutaDTO>>();
        try
        {
            var list = _unitOfWork.AccesoRutas.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<AccesoRutaDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "AccesoRuta encontrado";
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
            response.Data = _unitOfWork.AccesoRutas.Count();
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

    public async Task<Response<bool>> InsertAsync(AccesoRutaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<AccesoRuta>(dto);
            response.Data = await _unitOfWork.AccesoRutas.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "AccesoRuta creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(AccesoRutaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<AccesoRuta>(dto);
            response.Data = await _unitOfWork.AccesoRutas.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "AccesoRuta modificado correctamente";
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
            response.Data = await _unitOfWork.AccesoRutas.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "AccesoRuta eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<AccesoRutaDTO>> GetAsync(int id)
    {
        var response = new Response<AccesoRutaDTO>();
        try
        {
            var entity = await _unitOfWork.AccesoRutas.GetAsync(id);
            response.Data = _mapper.Map<AccesoRutaDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "AccesoRuta encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<AccesoRutaDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<AccesoRutaDTO>>();
        try
        {
            var list = await _unitOfWork.AccesoRutas.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<AccesoRutaDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "AccesoRuta encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<AccesoRutaDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<AccesoRutaDTO>>();
        try
        {
            var list = await _unitOfWork.AccesoRutas.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<AccesoRutaDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "AccesoRuta encontrado";
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
            response.Data = await _unitOfWork.AccesoRutas.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "AccesoRuta encontrado";
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