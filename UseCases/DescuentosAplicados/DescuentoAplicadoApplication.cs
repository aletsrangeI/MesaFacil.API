using Interface.Mapping;
using Common;
using Domain.Entities;
using DTO.DescuentoAplicado;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.DescuentosAplicados;

public class DescuentoAplicadoApplication : IDescuentoAplicadoApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppMapper _mapper;
    private readonly DescuentoAplicadoDTOValidator _validationRules;
    private readonly IAppLogger<DescuentoAplicadoApplication> _logger;

    public DescuentoAplicadoApplication(
        IUnitOfWork unitOfWork,
        IAppMapper mapper,
        DescuentoAplicadoDTOValidator validationRules,
        IAppLogger<DescuentoAplicadoApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(DescuentoAplicadoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<DescuentoAplicado>(dto);
            response.Data = _unitOfWork.DescuentosAplicados.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "DescuentoAplicado creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(DescuentoAplicadoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<DescuentoAplicado>(dto);
            response.Data = _unitOfWork.DescuentosAplicados.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "DescuentoAplicado modificado correctamente";
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
            response.Data = _unitOfWork.DescuentosAplicados.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "DescuentoAplicado eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<DescuentoAplicadoDTO> Get(int id)
    {
        var response = new Response<DescuentoAplicadoDTO>();
        try
        {
            var entity = _unitOfWork.DescuentosAplicados.Get(id);
            response.Data = _mapper.Map<DescuentoAplicadoDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "DescuentoAplicado encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<DescuentoAplicadoDTO>> GetAll()
    {
        var response = new Response<IEnumerable<DescuentoAplicadoDTO>>();
        try
        {
            var list = _unitOfWork.DescuentosAplicados.GetAll();
            response.Data = _mapper.Map<IEnumerable<DescuentoAplicadoDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "DescuentoAplicado encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<DescuentoAplicadoDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<DescuentoAplicadoDTO>>();
        try
        {
            var list = _unitOfWork.DescuentosAplicados.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<DescuentoAplicadoDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "DescuentoAplicado encontrado";
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
            response.Data = _unitOfWork.DescuentosAplicados.Count();
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

    public async Task<Response<bool>> InsertAsync(DescuentoAplicadoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<DescuentoAplicado>(dto);
            response.Data = await _unitOfWork.DescuentosAplicados.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "DescuentoAplicado creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(DescuentoAplicadoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<DescuentoAplicado>(dto);
            response.Data = await _unitOfWork.DescuentosAplicados.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "DescuentoAplicado modificado correctamente";
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
            response.Data = await _unitOfWork.DescuentosAplicados.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "DescuentoAplicado eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<DescuentoAplicadoDTO>> GetAsync(int id)
    {
        var response = new Response<DescuentoAplicadoDTO>();
        try
        {
            var entity = await _unitOfWork.DescuentosAplicados.GetAsync(id);
            response.Data = _mapper.Map<DescuentoAplicadoDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "DescuentoAplicado encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<DescuentoAplicadoDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<DescuentoAplicadoDTO>>();
        try
        {
            var list = await _unitOfWork.DescuentosAplicados.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<DescuentoAplicadoDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "DescuentoAplicado encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<DescuentoAplicadoDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<DescuentoAplicadoDTO>>();
        try
        {
            var list = await _unitOfWork.DescuentosAplicados.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<DescuentoAplicadoDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "DescuentoAplicado encontrado";
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
            response.Data = await _unitOfWork.DescuentosAplicados.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "DescuentoAplicado encontrado";
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