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
            var validation = _validationRules.Validate(dto);
            if (!validation.IsValid)
            {
                response.isSuccess = false;
                response.Message = "Errores de validación";
                response.Errors = validation.Errors;
                return response;
            }

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
            response.isSuccess = false;
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
            var validation = _validationRules.Validate(dto);
            if (!validation.IsValid)
            {
                response.isSuccess = false;
                response.Message = "Errores de validación";
                response.Errors = validation.Errors;
                return response;
            }

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
            response.isSuccess = false;
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
            else
            {
                response.isSuccess = false;
                response.Message = "No se pudo eliminar, el usuario no existe";
            }
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
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
            
            if (entity != null)
            {
                response.Data = _mapper.Map<UsuarioDTO>(entity);
                response.isSuccess = true;
                response.Message = "Usuario encontrado";
            }
            else
            {
                response.isSuccess = false;
                response.Message = "Usuario no encontrado";
            }
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
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
            response.isSuccess = true;
            response.Message = "Usuarios obtenidos correctamente";
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
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
            var count = _unitOfWork.Usuarios.Count();
            var list = _unitOfWork.Usuarios.GetAllWithPagination(page, pageSize);

            response.Data = _mapper.Map<IEnumerable<UsuarioDTO>>(list);
            response.PageNumber = page;
            response.TotalCount = count;
            response.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            response.isSuccess = true;
            response.Message = "Usuarios paginados obtenidos correctamente";
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
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
            response.Message = "Conteo obtenido correctamente";
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
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
            var validation = await _validationRules.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                response.isSuccess = false;
                response.Message = "Errores de validación";
                response.Errors = validation.Errors;
                return response;
            }

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
            response.isSuccess = false;
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
            var validation = await _validationRules.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                response.isSuccess = false;
                response.Message = "Errores de validación";
                response.Errors = validation.Errors;
                return response;
            }

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
            response.isSuccess = false;
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
            else
            {
                response.isSuccess = false;
                response.Message = "No se pudo eliminar, el usuario no existe";
            }
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
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
            
            if (entity != null)
            {
                response.Data = _mapper.Map<UsuarioDTO>(entity);
                response.isSuccess = true;
                response.Message = "Usuario encontrado";
            }
            else
            {
                response.isSuccess = false;
                response.Message = "Usuario no encontrado";
            }
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
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
            response.isSuccess = true;
            response.Message = "Usuarios obtenidos correctamente";
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
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
            var count = await _unitOfWork.Usuarios.CountAsync();
            var list = await _unitOfWork.Usuarios.GetAllWithPaginationAsync(page, pageSize);

            response.Data = _mapper.Map<IEnumerable<UsuarioDTO>>(list);
            response.PageNumber = page;
            response.TotalCount = count;
            response.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            response.isSuccess = true;
            response.Message = "Usuarios paginados obtenidos correctamente";
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
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
            response.isSuccess = true;
            response.Message = "Conteo obtenido correctamente";
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    #endregion
    
    #region Metodos de Seguridad y Operativos

    public async Task<Response<UsuarioDTO?>> GetByCorreoWithRolesAndCredentialsAsync(string correo, CancellationToken ct)
    {
        var response = new Response<UsuarioDTO?>();
        try
        {
            var entity = await _unitOfWork.Usuarios.GetByCorreoWithRolesAndCredentialsAsync(correo, ct);
            response.Data = _mapper.Map<UsuarioDTO?>(entity);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<UsuarioDTO?>> GetByUserOrEmailWithAuthGraphAsync(string userOrEmail, CancellationToken ct)
    {
        var response = new Response<UsuarioDTO?>();
        try
        {
            var entity = await _unitOfWork.Usuarios.GetByUserOrEmailWithAuthGraphAsync(userOrEmail, ct);
            response.Data = _mapper.Map<UsuarioDTO?>(entity);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IReadOnlyList<string>>> GetRoleNamesAsync(int usuarioId, CancellationToken ct)
    {
        var response = new Response<IReadOnlyList<string>>();
        try
        {
            response.Data = await _unitOfWork.Usuarios.GetRoleNamesAsync(usuarioId, ct);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IReadOnlyList<string>>> GetAccesoPathsByUsuarioIdAsync(int usuarioId, CancellationToken ct)
    {
        var response = new Response<IReadOnlyList<string>>();
        try
        {
            response.Data = await _unitOfWork.Usuarios.GetAccesoPathsByUsuarioIdAsync(usuarioId, ct);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<object?>> GetPasswordCredentialAsync(int usuarioId, CancellationToken ct)
    {
        var response = new Response<object?>();
        try
        {
            var entity = await _unitOfWork.Usuarios.GetPasswordCredentialAsync(usuarioId, ct);
            response.Data = _mapper.Map<object?>(entity); // Mapear a CredencialDTO si existe
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> HasOpenTurnoAsync(int idUsuario, CancellationToken ct)
    {
        var response = new Response<bool>();
        try
        {
            response.Data = await _unitOfWork.Usuarios.HasOpenTurnoAsync(idUsuario, ct);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<List<string>>> GetPermissionKeysByUsuarioIdAsync(int usuarioId, CancellationToken ct)
    {
        var response = new Response<List<string>>();
        try
        {
            response.Data = await _unitOfWork.Usuarios.GetPermissionKeysByUsuarioIdAsync(usuarioId, ct);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<string?>> GetPermissionsVersionAsync(int usuarioId, CancellationToken ct)
    {
        var response = new Response<string?>();
        try
        {
            response.Data = await _unitOfWork.Usuarios.GetPermissionsVersionAsync(usuarioId, ct);
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