using AutoMapper;
using Common;
using Domain.Entities;
using DTO.Usuario;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.Usuarios;

public class UsuarioApplication : IUsuarioApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly UsuarioDTOValidator _validationRules;
    private readonly IAppLogger<UsuarioApplication> _logger;

    public UsuarioApplication(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        UsuarioDTOValidator validationRules,
        IAppLogger<UsuarioApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(UsuarioDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Usuario>(dto);
            response.Data = _unitOfWork.Usuarios.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Usuario creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(UsuarioDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Usuario>(dto);
            response.Data = _unitOfWork.Usuarios.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Usuario modificado correctamente";
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
            response.Data = _unitOfWork.Usuarios.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Usuario eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<UsuarioDTO> Get(int id)
    {
        var response = new Response<UsuarioDTO>();
        try
        {
            var entity = _unitOfWork.Usuarios.Get(id);
            response.Data = _mapper.Map<UsuarioDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Usuario encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<UsuarioDTO>> GetAll()
    {
        var response = new Response<IEnumerable<UsuarioDTO>>();
        try
        {
            var list = _unitOfWork.Usuarios.GetAll();
            response.Data = _mapper.Map<IEnumerable<UsuarioDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Usuario encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<UsuarioDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<UsuarioDTO>>();
        try
        {
            var list = _unitOfWork.Usuarios.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<UsuarioDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "Usuario encontrado";
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
            response.Data = _unitOfWork.Usuarios.Count();
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

    public async Task<Response<bool>> InsertAsync(UsuarioDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Usuario>(dto);
            response.Data = await _unitOfWork.Usuarios.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Usuario creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(UsuarioDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Usuario>(dto);
            response.Data = await _unitOfWork.Usuarios.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Usuario modificado correctamente";
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
            response.Data = await _unitOfWork.Usuarios.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Usuario eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<UsuarioDTO>> GetAsync(int id)
    {
        var response = new Response<UsuarioDTO>();
        try
        {
            var entity = await _unitOfWork.Usuarios.GetAsync(id);
            response.Data = _mapper.Map<UsuarioDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Usuario encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<UsuarioDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<UsuarioDTO>>();
        try
        {
            var list = await _unitOfWork.Usuarios.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<UsuarioDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Usuario encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<UsuarioDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<UsuarioDTO>>();
        try
        {
            var list = await _unitOfWork.Usuarios.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<UsuarioDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "Usuario encontrado";
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
            response.Data = await _unitOfWork.Usuarios.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Usuario encontrado";
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