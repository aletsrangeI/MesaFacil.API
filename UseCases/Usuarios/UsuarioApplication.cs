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
                response.Message = "Errores de validación";
                response.Errors = validation.Errors;
                return response;
            }

            var entity = _mapper.Map<Usuario>(dto);
            response.Data = true;

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Usuario creado correctamente revisar esto despues";
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
            var validation = _validationRules.Validate(dto);
            if (!validation.IsValid)
            {
                response.Message = "Errores de validación";
                response.Errors = validation.Errors;
                return response;
            }

            var entity = _mapper.Map<Usuario>(dto);
            response.Data = true;

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Usuario modificado correctamente revisar esto despues";
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
            response.Data = true;

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Usuario eliminado correctamente revisar esto despues";
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
            var entity = new Usuario();
            response.Data = _mapper.Map<UsuarioDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Usuario encontrado revisar esto despues";
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
            var list = new List<Usuario>();
            response.Data = _mapper.Map<IEnumerable<UsuarioDTO>>(list);
            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Usuarios obtenidos correctamente revisar esto despues";
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
            var count = -1;

            response.Data = null;
            response.PageNumber = page;
            response.TotalCount = count;
            response.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            response.isSuccess = true;
            response.Message = "Revisar esto";
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
            response.Data = -1;
            response.isSuccess = true;
            response.Message = "Revisar esto";
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
            var validation = await _validationRules.ValidateAsync(dto);
            if (!validation.IsValid)
            {
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
                response.Message = "Usuarios obtenidos correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Task<ResponsePagination<IEnumerable<UsuarioDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        throw new NotImplementedException();
    }

    public Task<Response<int>> CountAsync()
    {
        throw new NotImplementedException();
    }

    #endregion
}