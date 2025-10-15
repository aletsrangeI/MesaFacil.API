using AutoMapper;
using Common;
using Domain.Entities;
using DTO.Area;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.Areas;

public class AreaApplication : IAreaApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly AreaDTOValidator _validationRules;
    private readonly IAppLogger<AreaApplication> _logger;

    public AreaApplication(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        AreaDTOValidator validationRules,
        IAppLogger<AreaApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(AreaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Area>(dto);
            response.Data = _unitOfWork.Areas.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Area creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<bool> Update(AreaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Area>(dto);
            response.Data = _unitOfWork.Areas.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Area modificado correctamente";
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
            response.Data = _unitOfWork.Areas.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Area eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<AreaDTO> Get(int id)
    {
        var response = new Response<AreaDTO>();
        try
        {
            var entity = _unitOfWork.Areas.Get(id);
            response.Data = _mapper.Map<AreaDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Area encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<IEnumerable<AreaDTO>> GetAll()
    {
        var response = new Response<IEnumerable<AreaDTO>>();
        try
        {
            var list = _unitOfWork.Areas.GetAll();
            response.Data = _mapper.Map<IEnumerable<AreaDTO>>(list);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public ResponsePagination<IEnumerable<AreaDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<AreaDTO>>();
        try
        {
            var catalogs = _unitOfWork.Catalogs.GetAllWithPagination(page, pageSize);
            response.Data = _mapper.Map<IEnumerable<AreaDTO>>(catalogs);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Areas encontradas";
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
            response.Data = _unitOfWork.Areas.Count();
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

    public async Task<Response<bool>> InsertAsync(AreaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Area>(dto);
            response.Data = await _unitOfWork.Areas.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Area creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<bool>> UpdateAsync(AreaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Area>(dto);
            response.Data = await _unitOfWork.Areas.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Area modificado correctamente";
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
            response.Data = await _unitOfWork.Areas.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Area eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<AreaDTO>> GetAsync(int id)
    {
        var response = new Response<AreaDTO>();
        try
        {
            var entity = await _unitOfWork.Areas.GetAsync(id);
            response.Data = _mapper.Map<AreaDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Area encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<IEnumerable<AreaDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<AreaDTO>>();
        try
        {
            var list = await _unitOfWork.Areas.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<AreaDTO>>(list);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<ResponsePagination<IEnumerable<AreaDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<AreaDTO>>();
        try
        {
            var list = await _unitOfWork.Areas.GetAllWithPaginationAsync(page, pageSize);
            response.Data = _mapper.Map<IEnumerable<AreaDTO>>(list);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Areas encontradas";
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
            response.Data = await _unitOfWork.Areas.CountAsync();
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