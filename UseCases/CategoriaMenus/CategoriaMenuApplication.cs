using Interface.Mapping;
using Common;
using Domain.Entities;
using DTO.CategoriaMenu;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.CategoriaMenus;

public class CategoriaMenuApplication : ICategoriaMenuApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppMapper _mapper;
    private readonly CategoriaMenuDTOValidator _validationRules;
    private readonly IAppLogger<CategoriaMenuApplication> _logger;

    public CategoriaMenuApplication(
        IUnitOfWork unitOfWork,
        IAppMapper mapper,
        CategoriaMenuDTOValidator validationRules,
        IAppLogger<CategoriaMenuApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(CategoriaMenuDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<CategoriaMenu>(dto);
            response.Data = _unitOfWork.CategoriaMenus.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "CategoriaMenu creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<bool> Update(CategoriaMenuDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<CategoriaMenu>(dto);
            response.Data = _unitOfWork.CategoriaMenus.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "CategoriaMenu modificado correctamente";
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
            response.Data = _unitOfWork.CategoriaMenus.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "CategoriaMenu eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<CategoriaMenuDTO> Get(int id)
    {
        var response = new Response<CategoriaMenuDTO>();
        try
        {
            var entity = _unitOfWork.CategoriaMenus.Get(id);
            response.Data = _mapper.Map<CategoriaMenuDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "CategoriaMenu encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<IEnumerable<CategoriaMenuDTO>> GetAll()
    {
        var response = new Response<IEnumerable<CategoriaMenuDTO>>();
        try
        {
            var list = _unitOfWork.CategoriaMenus.GetAll();
            response.Data = _mapper.Map<IEnumerable<CategoriaMenuDTO>>(list);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public ResponsePagination<IEnumerable<CategoriaMenuDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<CategoriaMenuDTO>>();
        try
        {
            var list = _unitOfWork.CategoriaMenus.GetAllWithPagination(page, pageSize);

            if (list != null)
            {
                response.Data = _mapper.Map<IEnumerable<CategoriaMenuDTO>>(list);
                response.isSuccess = true;
                response.Message = "CategoriaMenu encontrado";
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
            response.Data = _unitOfWork.CategoriaMenus.Count();
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

    public async Task<Response<bool>> InsertAsync(CategoriaMenuDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<CategoriaMenu>(dto);
            response.Data = await _unitOfWork.CategoriaMenus.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "CategoriaMenu creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<bool>> UpdateAsync(CategoriaMenuDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<CategoriaMenu>(dto);
            response.Data = await _unitOfWork.CategoriaMenus.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "CategoriaMenu modificado correctamente";
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
            response.Data = await _unitOfWork.CategoriaMenus.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "CategoriaMenu eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<CategoriaMenuDTO>> GetAsync(int id)
    {
        var response = new Response<CategoriaMenuDTO>();
        try
        {
            var entity = await _unitOfWork.CategoriaMenus.GetAsync(id);
            response.Data = _mapper.Map<CategoriaMenuDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "CategoriaMenu encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<IEnumerable<CategoriaMenuDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<CategoriaMenuDTO>>();
        try
        {
            var list = await _unitOfWork.CategoriaMenus.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<CategoriaMenuDTO>>(list);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<ResponsePagination<IEnumerable<CategoriaMenuDTO>>> GetAllWithPaginationAsync(int page,
        int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<CategoriaMenuDTO>>();
        try
        {
            var list = await _unitOfWork.CategoriaMenus.GetAllWithPaginationAsync(page, pageSize);

            if (list != null)
            {
                response.Data = _mapper.Map<IEnumerable<CategoriaMenuDTO>>(list);
                response.isSuccess = true;
                response.Message = "CategoriaMenu encontrado";
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
            response.Data = await _unitOfWork.CategoriaMenus.CountAsync();
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