using Interface.Mapping;
using Common;
using Domain.Entities;
using DTO.Precio;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.Precios;

public class PrecioApplication : IPrecioApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppMapper _mapper;
    private readonly PrecioDTOValidator _validationRules;
    private readonly IAppLogger<PrecioApplication> _logger;

    public PrecioApplication(
        IUnitOfWork unitOfWork,
        IAppMapper mapper,
        PrecioDTOValidator validationRules,
        IAppLogger<PrecioApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(PrecioDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Precio>(dto);
            entity.Moneda ??= "MXN";
            response.Data = _unitOfWork.Precios.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Precio creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(PrecioDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Precio>(dto);
            entity.Moneda ??= "MXN";
            response.Data = _unitOfWork.Precios.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Precio modificado correctamente";
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
            response.Data = _unitOfWork.Precios.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Precio eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<PrecioDTO> Get(int id)
    {
        var response = new Response<PrecioDTO>();
        try
        {
            var entity = _unitOfWork.Precios.Get(id);
            response.Data = _mapper.Map<PrecioDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Precio encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<PrecioDTO>> GetAll()
    {
        var response = new Response<IEnumerable<PrecioDTO>>();
        try
        {
            var list = _unitOfWork.Precios.GetAll();
            response.Data = _mapper.Map<IEnumerable<PrecioDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Precio encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<PrecioDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<PrecioDTO>>();
        try
        {
            var list = _unitOfWork.Precios.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<PrecioDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "Precio encontrado";
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
            response.Data = _unitOfWork.Precios.Count();
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

    public async Task<Response<bool>> InsertAsync(PrecioDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Precio>(dto);
            entity.Moneda ??= "MXN";
            response.Data = await _unitOfWork.Precios.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Precio creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(PrecioDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Precio>(dto);
            entity.Moneda ??= "MXN";
            response.Data = await _unitOfWork.Precios.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Precio modificado correctamente";
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
            response.Data = await _unitOfWork.Precios.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Precio eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<PrecioDTO>> GetAsync(int id)
    {
        var response = new Response<PrecioDTO>();
        try
        {
            var entity = await _unitOfWork.Precios.GetAsync(id);
            response.Data = _mapper.Map<PrecioDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Precio encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<PrecioDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<PrecioDTO>>();
        try
        {
            var list = await _unitOfWork.Precios.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<PrecioDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Precio encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<PrecioDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<PrecioDTO>>();
        try
        {
            var list = await _unitOfWork.Precios.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<PrecioDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "Precio encontrado";
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
            response.Data = await _unitOfWork.Precios.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Precio encontrado";
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