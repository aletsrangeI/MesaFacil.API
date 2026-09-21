using Interface.Mapping;
using Common;
using Domain.Entities;
using DTO.UsuarioRol;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.UsuarioRoles;

public class UsuarioRolApplication : IUsuarioRolApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppMapper _mapper;
    private readonly UsuarioRolDTOValidator _validationRules;
    private readonly IAppLogger<UsuarioRolApplication> _logger;

    public UsuarioRolApplication(
        IUnitOfWork unitOfWork,
        IAppMapper mapper,
        UsuarioRolDTOValidator validationRules,
        IAppLogger<UsuarioRolApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(UsuarioRolDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<UsuarioRol>(dto);
            response.Data = _unitOfWork.UsuarioRoles.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "UsuarioRol creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(UsuarioRolDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<UsuarioRol>(dto);
            response.Data = _unitOfWork.UsuarioRoles.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "UsuarioRol modificado correctamente";
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
            response.Data = _unitOfWork.UsuarioRoles.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "UsuarioRol eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<UsuarioRolDTO> Get(int id)
    {
        var response = new Response<UsuarioRolDTO>();
        try
        {
            var entity = _unitOfWork.UsuarioRoles.Get(id);
            response.Data = _mapper.Map<UsuarioRolDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "UsuarioRol encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<UsuarioRolDTO>> GetAll()
    {
        var response = new Response<IEnumerable<UsuarioRolDTO>>();
        try
        {
            var list = _unitOfWork.UsuarioRoles.GetAll();
            response.Data = _mapper.Map<IEnumerable<UsuarioRolDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "UsuarioRol encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<UsuarioRolDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<UsuarioRolDTO>>();
        try
        {
            var list = _unitOfWork.UsuarioRoles.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<UsuarioRolDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "UsuarioRol encontrado";
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
            response.Data = _unitOfWork.UsuarioRoles.Count();
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

    public async Task<Response<bool>> InsertAsync(UsuarioRolDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<UsuarioRol>(dto);
            response.Data = await _unitOfWork.UsuarioRoles.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "UsuarioRol creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(UsuarioRolDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<UsuarioRol>(dto);
            response.Data = await _unitOfWork.UsuarioRoles.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "UsuarioRol modificado correctamente";
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
            response.Data = await _unitOfWork.UsuarioRoles.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "UsuarioRol eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<UsuarioRolDTO>> GetAsync(int id)
    {
        var response = new Response<UsuarioRolDTO>();
        try
        {
            var entity = await _unitOfWork.UsuarioRoles.GetAsync(id);
            response.Data = _mapper.Map<UsuarioRolDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "UsuarioRol encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<UsuarioRolDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<UsuarioRolDTO>>();
        try
        {
            var list = await _unitOfWork.UsuarioRoles.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<UsuarioRolDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "UsuarioRol encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<UsuarioRolDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<UsuarioRolDTO>>();
        try
        {
            var list = await _unitOfWork.UsuarioRoles.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<UsuarioRolDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "UsuarioRol encontrado";
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
            response.Data = await _unitOfWork.UsuarioRoles.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "UsuarioRol encontrado";
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