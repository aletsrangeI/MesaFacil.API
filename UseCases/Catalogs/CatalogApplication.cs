using AutoMapper;
using Common;
using Domain.Entities;
using DTO.Catalog;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.Catalogs;

public class CatalogApplication : ICatalogApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly CatalogDTOValidator _validationRules;
    private readonly IAppLogger<CatalogApplication> _logger;

    public CatalogApplication(IUnitOfWork unitOfWork, IMapper mapper, CatalogDTOValidator validationRules,
        IAppLogger<CatalogApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos Sincronos

    public Response<bool> Insert(CatalogDTO catalogDTO)
    {
        var response = new Response<bool>();
        try
        {
            var catalog = _mapper.Map<Catalog>(catalogDTO);
            response.Data = _unitOfWork.Catalogs.Insert(catalog);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "El catalogo ha sido creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<bool> Update(CatalogDTO catalogDTO)
    {
        var response = new Response<bool>();

        try
        {
            var catalog = _mapper.Map<Catalog>(catalogDTO);
            response.Data = _unitOfWork.Catalogs.Update(catalog);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "El catalogo ha sido modificado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<bool> Delete(int catalogId)
    {
        var response = new Response<bool>();

        try
        {
            response.Data = _unitOfWork.Catalogs.Delete(catalogId);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "El catalogo ha sido eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<CatalogDTO> Get(int catalogId)
    {
        var response = new Response<CatalogDTO>();

        try
        {
            var catalog = _unitOfWork.Catalogs.Get(catalogId);
            response.Data = _mapper.Map<CatalogDTO>(catalog);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Catalogo encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<IEnumerable<CatalogDTO>> GetAll()
    {
        var response = new Response<IEnumerable<CatalogDTO>>();

        try
        {
            var catalogs = _unitOfWork.Catalogs.GetAll();
            response.Data = _mapper.Map<IEnumerable<CatalogDTO>>(catalogs);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Catalogos encontrados";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public ResponsePagination<IEnumerable<CatalogDTO>> GetAllWithPagination(int pageNumber, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<CatalogDTO>>();

        try
        {
            var catalogs = _unitOfWork.Catalogs.GetAllWithPagination(pageNumber, pageSize);
            response.Data = _mapper.Map<IEnumerable<CatalogDTO>>(catalogs);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Catalogos encontrados";
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
            response.Data = _unitOfWork.Catalogs.Count();

            if (response.Data > 0)
            {
                response.isSuccess = true;
                response.Message = "Numero de catalogos encontrados";
            }
        }
        catch (Exception e)
        {
            response.Message = e.Message;
            _logger.LogError(e.Message);
        }

        return response;
    }

    #endregion

    #region Metodos asincronos

    public async Task<Response<bool>> InsertAsync(CatalogDTO catalogDTO)
    {
        var response = new Response<bool>();

        try
        {
            var catalog = _mapper.Map<Catalog>(catalogDTO);
            response.Data = await _unitOfWork.Catalogs.InsertAsync(catalog);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "El catalogo ha sido creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<bool>> UpdateAsync(CatalogDTO catalogDTO)
    {
        var response = new Response<bool>();

        try
        {
            var catalog = _mapper.Map<Catalog>(catalogDTO);
            response.Data = await _unitOfWork.Catalogs.UpdateAsync(catalog);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "El catalogo ha sido modificado correctamente";
            }
        }
        catch (Exception e)
        {
            response.Message = e.Message;
            _logger.LogError(e.Message);
        }

        return response;
    }

    public async Task<Response<bool>> DeleteAsync(int id)
    {
        var response = new Response<bool>();

        try
        {
            response.Data = await _unitOfWork.Catalogs.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "El catalogo ha sido eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<CatalogDTO>> GetAsync(int id)
    {
        var response = new Response<CatalogDTO>();

        try
        {
            var catalog = await _unitOfWork.Catalogs.GetAsync(id);
            response.Data = _mapper.Map<CatalogDTO>(catalog);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Catalogo encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<IEnumerable<CatalogDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<CatalogDTO>>();

        try
        {
            var catalogs = await _unitOfWork.Catalogs.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<CatalogDTO>>(catalogs);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Catalogos encontrados";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<ResponsePagination<IEnumerable<CatalogDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<CatalogDTO>>();

        try
        {
            var catalogs = await _unitOfWork.Catalogs.GetAllWithPaginationAsync(page, pageSize);
            response.Data = _mapper.Map<IEnumerable<CatalogDTO>>(catalogs);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Catalogos encontrados";
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
            response.Data = await _unitOfWork.Catalogs.CountAsync();
            if (response.Data > 0)
            {
                response.isSuccess = true;
                response.Message = "Numero de catalogos encontrados";
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