using AutoMapper;
using Common;
using Domain.Entities;
using DTO.Rol;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.Roles;

public class RolApplication : IRolApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly RolDTOValidator _validationRules;
    private readonly IAppLogger<RolApplication> _logger;

    public RolApplication(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        RolDTOValidator validationRules,
        IAppLogger<RolApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(RolDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Rol>(dto);
            response.Data = _unitOfWork.Roles.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Rol creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(RolDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Rol>(dto);
            response.Data = _unitOfWork.Roles.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Rol modificado correctamente";
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
            response.Data = _unitOfWork.Roles.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Rol eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<RolDTO> Get(int id)
    {
        var response = new Response<RolDTO>();
        try
        {
            var entity = _unitOfWork.Roles.Get(id);
            response.Data = _mapper.Map<RolDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Rol encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<RolDTO>> GetAll()
    {
        var response = new Response<IEnumerable<RolDTO>>();
        try
        {
            var list = _unitOfWork.Roles.GetAll();
            response.Data = _mapper.Map<IEnumerable<RolDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Rol encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<RolDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<RolDTO>>();
        try
        {
            var list = _unitOfWork.Roles.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<RolDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "Rol encontrado";
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
            response.Data = _unitOfWork.Roles.Count();
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

    public async Task<Response<bool>> InsertAsync(RolDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Rol>(dto);
            response.Data = await _unitOfWork.Roles.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Rol creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(RolDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Rol>(dto);
            response.Data = await _unitOfWork.Roles.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Rol modificado correctamente";
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
            response.Data = await _unitOfWork.Roles.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Rol eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<RolDTO>> GetAsync(int id)
    {
        var response = new Response<RolDTO>();
        try
        {
            var entity = await _unitOfWork.Roles.GetAsync(id);
            response.Data = _mapper.Map<RolDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Rol encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<RolDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<RolDTO>>();
        try
        {
            var list = await _unitOfWork.Roles.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<RolDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Rol encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<RolDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<RolDTO>>();
        try
        {
            var list = await _unitOfWork.Roles.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<RolDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "Rol encontrado";
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
            response.Data = await _unitOfWork.Roles.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Rol encontrado";
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