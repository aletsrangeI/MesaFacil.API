using AutoMapper;
using Common;
using Domain.Entities;
using DTO.Menu;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.Menus;

public class MenuApplication : IMenuApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly MenuDTOValidator _validationRules;
    private readonly IAppLogger<MenuApplication> _logger;

    public MenuApplication(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        MenuDTOValidator validationRules,
        IAppLogger<MenuApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(MenuDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Menu>(dto);
            response.Data = _unitOfWork.Menus.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Menu creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(MenuDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Menu>(dto);
            response.Data = _unitOfWork.Menus.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Menu modificado correctamente";
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
            response.Data = _unitOfWork.Menus.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Menu eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<MenuDTO> Get(int id)
    {
        var response = new Response<MenuDTO>();
        try
        {
            var entity = _unitOfWork.Menus.Get(id);
            response.Data = _mapper.Map<MenuDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Menu encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<MenuDTO>> GetAll()
    {
        var response = new Response<IEnumerable<MenuDTO>>();
        try
        {
            var list = _unitOfWork.Menus.GetAll();
            response.Data = _mapper.Map<IEnumerable<MenuDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Menu encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<MenuDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<MenuDTO>>();
        try
        {
            var list = _unitOfWork.Menus.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<MenuDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "Menu encontrado";
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
            response.Data = _unitOfWork.Menus.Count();
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

    public async Task<Response<bool>> InsertAsync(MenuDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Menu>(dto);
            response.Data = await _unitOfWork.Menus.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Menu creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(MenuDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Menu>(dto);
            response.Data = await _unitOfWork.Menus.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Menu modificado correctamente";
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
            response.Data = await _unitOfWork.Menus.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Menu eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<MenuDTO>> GetAsync(int id)
    {
        var response = new Response<MenuDTO>();
        try
        {
            var entity = await _unitOfWork.Menus.GetAsync(id);
            response.Data = _mapper.Map<MenuDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Menu encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<MenuDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<MenuDTO>>();
        try
        {
            var list = await _unitOfWork.Menus.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<MenuDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Menu encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<MenuDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<MenuDTO>>();
        try
        {
            var list = await _unitOfWork.Menus.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<MenuDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "Menu encontrado";
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
            response.Data = await _unitOfWork.Menus.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Menu encontrado";
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