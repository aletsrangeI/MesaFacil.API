using AutoMapper;
using Common;
using Domain.Entities;
using DTO.GenericCatalog;
using Interface.Persistence;
using Interface.UseCases;

namespace UseCases.Common;

/// <summary>
/// Implementación genérica del CRUD para cualquier catálogo simple.
/// TEntity debe heredar de BaseAuditableEntity e implementar ICatalogEntity.
/// Una sola clase reemplaza los 13 XxxApplication de catálogos individuales.
/// </summary>
public class GenericCatalogApplication<TEntity> : IGenericCatalogApplication
    where TEntity : BaseAuditableEntity, ICatalogEntity, new()
{
    private readonly IGenericRepository<TEntity>               _repo;
    private readonly IMapper                                   _mapper;
    private readonly IAppLogger<GenericCatalogApplication<TEntity>> _logger;

    public GenericCatalogApplication(
        IGenericRepository<TEntity>               repo,
        IMapper                                   mapper,
        IAppLogger<GenericCatalogApplication<TEntity>> logger)
    {
        _repo   = repo;
        _mapper = mapper;
        _logger = logger;
    }

    #region Métodos síncronos

    public Response<bool> Insert(GenericCatalogDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity       = _mapper.Map<TEntity>(dto);
            response.Data    = _repo.Insert(entity);
            response.isSuccess = response.Data;
            response.Message   = response.Data
                ? $"{typeof(TEntity).Name} creado correctamente"
                : $"No se pudo crear {typeof(TEntity).Name}";
        }
        catch (Exception ex) { response.Message = ex.Message; _logger.LogError(ex.Message); }
        return response;
    }

    public Response<bool> Update(GenericCatalogDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity       = _mapper.Map<TEntity>(dto);
            response.Data    = _repo.Update(entity);
            response.isSuccess = response.Data;
            response.Message   = response.Data
                ? $"{typeof(TEntity).Name} modificado correctamente"
                : $"No se pudo modificar {typeof(TEntity).Name}";
        }
        catch (Exception ex) { response.Message = ex.Message; _logger.LogError(ex.Message); }
        return response;
    }

    public Response<bool> Delete(int id)
    {
        var response = new Response<bool>();
        try
        {
            response.Data      = _repo.Delete(id);
            response.isSuccess = response.Data;
            response.Message   = response.Data
                ? $"{typeof(TEntity).Name} eliminado correctamente"
                : $"{typeof(TEntity).Name} no encontrado";
        }
        catch (Exception ex) { response.Message = ex.Message; _logger.LogError(ex.Message); }
        return response;
    }

    public Response<GenericCatalogDTO> Get(int id)
    {
        var response = new Response<GenericCatalogDTO>();
        try
        {
            var entity     = _repo.Get(id);
            response.Data  = _mapper.Map<GenericCatalogDTO>(entity);
            response.isSuccess = response.Data != null;
            response.Message   = response.isSuccess
                ? $"{typeof(TEntity).Name} encontrado"
                : $"{typeof(TEntity).Name} no encontrado";
        }
        catch (Exception ex) { response.Message = ex.Message; _logger.LogError(ex.Message); }
        return response;
    }

    public Response<IEnumerable<GenericCatalogDTO>> GetAll()
    {
        var response = new Response<IEnumerable<GenericCatalogDTO>>();
        try
        {
            var list       = _repo.GetAll();
            response.Data  = _mapper.Map<IEnumerable<GenericCatalogDTO>>(list);
            response.isSuccess = true;
            response.Message   = "OK";
        }
        catch (Exception ex) { response.Message = ex.Message; _logger.LogError(ex.Message); }
        return response;
    }

    public ResponsePagination<IEnumerable<GenericCatalogDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<GenericCatalogDTO>>();
        try
        {
            var list       = _repo.GetAllWithPagination(page, pageSize);
            response.Data  = _mapper.Map<IEnumerable<GenericCatalogDTO>>(list);
            response.isSuccess = true;
            response.Message   = "OK";
        }
        catch (Exception ex) { response.Message = ex.Message; _logger.LogError(ex.Message); }
        return response;
    }

    public Response<int> Count()
    {
        var response = new Response<int>();
        try
        {
            response.Data      = _repo.Count();
            response.isSuccess = true;
        }
        catch (Exception ex) { response.Message = ex.Message; _logger.LogError(ex.Message); }
        return response;
    }

    #endregion

    #region Métodos asíncronos

    public async Task<Response<bool>> InsertAsync(GenericCatalogDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity       = _mapper.Map<TEntity>(dto);
            response.Data    = await _repo.InsertAsync(entity);
            response.isSuccess = response.Data;
            response.Message   = response.Data
                ? $"{typeof(TEntity).Name} creado correctamente"
                : $"No se pudo crear {typeof(TEntity).Name}";
        }
        catch (Exception ex) { response.Message = ex.Message; _logger.LogError(ex.Message); }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(GenericCatalogDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity       = _mapper.Map<TEntity>(dto);
            response.Data    = await _repo.UpdateAsync(entity);
            response.isSuccess = response.Data;
            response.Message   = response.Data
                ? $"{typeof(TEntity).Name} modificado correctamente"
                : $"No se pudo modificar {typeof(TEntity).Name}";
        }
        catch (Exception ex) { response.Message = ex.Message; _logger.LogError(ex.Message); }
        return response;
    }

    public async Task<Response<bool>> DeleteAsync(int id)
    {
        var response = new Response<bool>();
        try
        {
            response.Data      = await _repo.DeleteAsync(id);
            response.isSuccess = response.Data;
            response.Message   = response.Data
                ? $"{typeof(TEntity).Name} eliminado correctamente"
                : $"{typeof(TEntity).Name} no encontrado";
        }
        catch (Exception ex) { response.Message = ex.Message; _logger.LogError(ex.Message); }
        return response;
    }

    public async Task<Response<GenericCatalogDTO>> GetAsync(int id)
    {
        var response = new Response<GenericCatalogDTO>();
        try
        {
            var entity     = await _repo.GetAsync(id);
            response.Data  = _mapper.Map<GenericCatalogDTO>(entity);
            response.isSuccess = response.Data != null;
            response.Message   = response.isSuccess
                ? $"{typeof(TEntity).Name} encontrado"
                : $"{typeof(TEntity).Name} no encontrado";
        }
        catch (Exception ex) { response.Message = ex.Message; _logger.LogError(ex.Message); }
        return response;
    }

    public async Task<Response<IEnumerable<GenericCatalogDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<GenericCatalogDTO>>();
        try
        {
            var list       = await _repo.GetAllAsync();
            response.Data  = _mapper.Map<IEnumerable<GenericCatalogDTO>>(list);
            response.isSuccess = true;
            response.Message   = "OK";
        }
        catch (Exception ex) { response.Message = ex.Message; _logger.LogError(ex.Message); }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<GenericCatalogDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<GenericCatalogDTO>>();
        try
        {
            var list       = await _repo.GetAllWithPaginationAsync(page, pageSize);
            response.Data  = _mapper.Map<IEnumerable<GenericCatalogDTO>>(list);
            response.isSuccess = true;
            response.Message   = "OK";
        }
        catch (Exception ex) { response.Message = ex.Message; _logger.LogError(ex.Message); }
        return response;
    }

    public async Task<Response<int>> CountAsync()
    {
        var response = new Response<int>();
        try
        {
            response.Data      = await _repo.CountAsync();
            response.isSuccess = true;
        }
        catch (Exception ex) { response.Message = ex.Message; _logger.LogError(ex.Message); }
        return response;
    }

    #endregion
}
