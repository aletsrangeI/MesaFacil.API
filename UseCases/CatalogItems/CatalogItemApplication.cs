using AutoMapper;
using Common;
using Domain.Entities;
using DTO.CatalogItem;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.CatalogItems;

public class CatalogItemApplication : ICatalogItemApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly CatalogItemDTOValidator _validationRules;
    private readonly IAppLogger<CatalogItemApplication> _logger;

    public CatalogItemApplication(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        CatalogItemDTOValidator validationRules,
        IAppLogger<CatalogItemApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(CatalogItemDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<CatalogItem>(dto);
            response.Data = _unitOfWork.CatalogItems.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "CatalogItem creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(CatalogItemDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<CatalogItem>(dto);
            response.Data = _unitOfWork.CatalogItems.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "CatalogItem modificado correctamente";
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
            response.Data = _unitOfWork.CatalogItems.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "CatalogItem eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<CatalogItemDTO> Get(int id)
    {
        var response = new Response<CatalogItemDTO>();
        try
        {
            var entity = _unitOfWork.CatalogItems.Get(id);
            response.Data = _mapper.Map<CatalogItemDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "CatalogItem encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<CatalogItemDTO>> GetAll()
    {
        var response = new Response<IEnumerable<CatalogItemDTO>>();
        try
        {
            var list = _unitOfWork.CatalogItems.GetAll();
            response.Data = _mapper.Map<IEnumerable<CatalogItemDTO>>(list);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<CatalogItemDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<CatalogItemDTO>>();
        try
        {
            var list = _unitOfWork.CatalogItems.GetAllWithPagination(page, pageSize);
            response.Data = _mapper.Map<IEnumerable<CatalogItemDTO>>(list);
            response.isSuccess = true;
            response.PageNumber = page;
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
            response.Data = _unitOfWork.CatalogItems.Count();
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

    public async Task<Response<bool>> InsertAsync(CatalogItemDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<CatalogItem>(dto);
            response.Data = await _unitOfWork.CatalogItems.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "CatalogItem creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(CatalogItemDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<CatalogItem>(dto);
            response.Data = await _unitOfWork.CatalogItems.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "CatalogItem modificado correctamente";
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
            response.Data = await _unitOfWork.CatalogItems.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "CatalogItem eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<CatalogItemDTO>> GetAsync(int id)
    {
        var response = new Response<CatalogItemDTO>();
        try
        {
            var entity = await _unitOfWork.CatalogItems.GetAsync(id);
            response.Data = _mapper.Map<CatalogItemDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "CatalogItem encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<CatalogItemDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<CatalogItemDTO>>();
        try
        {
            var list = await _unitOfWork.CatalogItems.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<CatalogItemDTO>>(list);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<CatalogItemDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<CatalogItemDTO>>();
        try
        {
            var list = await _unitOfWork.CatalogItems.GetAllWithPaginationAsync(page, pageSize);
            response.Data = _mapper.Map<IEnumerable<CatalogItemDTO>>(list);
            response.isSuccess = true;
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
            response.Data = await _unitOfWork.CatalogItems.CountAsync();
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
}