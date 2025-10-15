using AutoMapper;
using Common;
using Domain.Entities;
using DTO.Producto;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.Productos;

public class ProductoApplication : IProductoApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ProductoDTOValidator _validationRules;
    private readonly IAppLogger<ProductoApplication> _logger;

    public ProductoApplication(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ProductoDTOValidator validationRules,
        IAppLogger<ProductoApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(ProductoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Producto>(dto);
            response.Data = _unitOfWork.Productos.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Producto creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(ProductoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Producto>(dto);
            response.Data = _unitOfWork.Productos.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Producto modificado correctamente";
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
            response.Data = _unitOfWork.Productos.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Producto eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<ProductoDTO> Get(int id)
    {
        var response = new Response<ProductoDTO>();
        try
        {
            var entity = _unitOfWork.Productos.Get(id);
            response.Data = _mapper.Map<ProductoDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Producto encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<ProductoDTO>> GetAll()
    {
        var response = new Response<IEnumerable<ProductoDTO>>();
        try
        {
            var list = _unitOfWork.Productos.GetAll();
            response.Data = _mapper.Map<IEnumerable<ProductoDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Producto encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<ProductoDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<ProductoDTO>>();
        try
        {
            var list = _unitOfWork.Productos.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<ProductoDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "Producto encontrado";
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
            response.Data = _unitOfWork.Productos.Count();
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

    public async Task<Response<bool>> InsertAsync(ProductoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Producto>(dto);
            response.Data = await _unitOfWork.Productos.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Producto creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(ProductoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Producto>(dto);
            response.Data = await _unitOfWork.Productos.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Producto modificado correctamente";
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
            response.Data = await _unitOfWork.Productos.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Producto eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<ProductoDTO>> GetAsync(int id)
    {
        var response = new Response<ProductoDTO>();
        try
        {
            var entity = await _unitOfWork.Productos.GetAsync(id);
            response.Data = _mapper.Map<ProductoDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Producto encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<ProductoDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<ProductoDTO>>();
        try
        {
            var list = await _unitOfWork.Productos.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<ProductoDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Producto encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<ProductoDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<ProductoDTO>>();
        try
        {
            var list = await _unitOfWork.Productos.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<ProductoDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "Producto encontrado";
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
            response.Data = await _unitOfWork.Productos.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Producto encontrado";
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