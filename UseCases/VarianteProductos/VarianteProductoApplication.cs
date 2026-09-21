using Interface.Mapping;
using Common;
using Domain.Entities;
using DTO.VarianteProducto;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.VarianteProductos;

public class VarianteProductoApplication : IVarianteProductoApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppMapper _mapper;
    private readonly VarianteProductoDTOValidator _validationRules;
    private readonly IAppLogger<VarianteProductoApplication> _logger;

    public VarianteProductoApplication(
        IUnitOfWork unitOfWork,
        IAppMapper mapper,
        VarianteProductoDTOValidator validationRules,
        IAppLogger<VarianteProductoApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(VarianteProductoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<VarianteProducto>(dto);
            response.Data = _unitOfWork.VarianteProductos.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "VarianteProducto creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(VarianteProductoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<VarianteProducto>(dto);
            response.Data = _unitOfWork.VarianteProductos.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "VarianteProducto modificado correctamente";
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
            response.Data = _unitOfWork.VarianteProductos.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "VarianteProducto eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<VarianteProductoDTO> Get(int id)
    {
        var response = new Response<VarianteProductoDTO>();
        try
        {
            var entity = _unitOfWork.VarianteProductos.Get(id);
            response.Data = _mapper.Map<VarianteProductoDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "VarianteProducto encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<VarianteProductoDTO>> GetAll()
    {
        var response = new Response<IEnumerable<VarianteProductoDTO>>();
        try
        {
            var list = _unitOfWork.VarianteProductos.GetAll();
            response.Data = _mapper.Map<IEnumerable<VarianteProductoDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "VarianteProducto encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<VarianteProductoDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<VarianteProductoDTO>>();
        try
        {
            var list = _unitOfWork.VarianteProductos.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<VarianteProductoDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "VarianteProducto encontrado";
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
            response.Data = _unitOfWork.VarianteProductos.Count();
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

    public async Task<Response<bool>> InsertAsync(VarianteProductoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<VarianteProducto>(dto);
            response.Data = await _unitOfWork.VarianteProductos.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "VarianteProducto creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(VarianteProductoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<VarianteProducto>(dto);
            response.Data = await _unitOfWork.VarianteProductos.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "VarianteProducto modificado correctamente";
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
            response.Data = await _unitOfWork.VarianteProductos.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "VarianteProducto eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<VarianteProductoDTO>> GetAsync(int id)
    {
        var response = new Response<VarianteProductoDTO>();
        try
        {
            var entity = await _unitOfWork.VarianteProductos.GetAsync(id);
            response.Data = _mapper.Map<VarianteProductoDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "VarianteProducto encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<VarianteProductoDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<VarianteProductoDTO>>();
        try
        {
            var list = await _unitOfWork.VarianteProductos.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<VarianteProductoDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "VarianteProducto encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<VarianteProductoDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<VarianteProductoDTO>>();
        try
        {
            var list = await _unitOfWork.VarianteProductos.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<VarianteProductoDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "VarianteProducto encontrado";
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
            response.Data = await _unitOfWork.VarianteProductos.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "VarianteProducto encontrado";
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