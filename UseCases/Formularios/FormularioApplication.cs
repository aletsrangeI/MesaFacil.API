using Interface.Mapping;
using Common;
using Domain.Entities;
using DTO.Formulario;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.Formularios;

public class FormularioApplication : IFormularioApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppMapper _mapper;
    private readonly FormularioDTOValidator _validationRules;
    private readonly IAppLogger<FormularioApplication> _logger;

    public FormularioApplication(
        IUnitOfWork unitOfWork,
        IAppMapper mapper,
        FormularioDTOValidator validationRules,
        IAppLogger<FormularioApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(FormularioDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Formulario>(dto);
            response.Data = _unitOfWork.Formularios.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Formulario creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(FormularioDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Formulario>(dto);
            response.Data = _unitOfWork.Formularios.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Formulario modificado correctamente";
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
            response.Data = _unitOfWork.Formularios.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Formulario eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<FormularioDTO> Get(int id)
    {
        var response = new Response<FormularioDTO>();
        try
        {
            var entity = _unitOfWork.Formularios.Get(id);
            response.Data = _mapper.Map<FormularioDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Formulario encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<FormularioDTO>> GetAll()
    {
        var response = new Response<IEnumerable<FormularioDTO>>();
        try
        {
            var list = _unitOfWork.Formularios.GetAll();
            response.Data = _mapper.Map<IEnumerable<FormularioDTO>>(list);
            
            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Formulario encontrados";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<FormularioDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<FormularioDTO>>();
        try
        {
            var list = _unitOfWork.Formularios.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
                response.Data = _mapper.Map<IEnumerable<FormularioDTO>>(list);
                response.isSuccess = true;
                response.Message = "Formulario encontrados";
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
            response.Data = _unitOfWork.Formularios.Count();
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

    public async Task<Response<bool>> InsertAsync(FormularioDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Formulario>(dto);
            response.Data = await _unitOfWork.Formularios.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Formulario creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(FormularioDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Formulario>(dto);
            response.Data = await _unitOfWork.Formularios.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Formulario modificado correctamente";
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
            response.Data = await _unitOfWork.Formularios.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Formulario eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<FormularioDTO>> GetAsync(int id)
    {
        var response = new Response<FormularioDTO>();
        try
        {
            var entity = await _unitOfWork.Formularios.GetAsync(id);
            response.Data = _mapper.Map<FormularioDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Formulario encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<FormularioDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<FormularioDTO>>();
        try
        {
            var list = await _unitOfWork.Formularios.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<FormularioDTO>>(list);
            
            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Formulario encontrados";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<FormularioDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<FormularioDTO>>();
        try
        {
            var list = await _unitOfWork.Formularios.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
                response.Data = _mapper.Map<IEnumerable<FormularioDTO>>(list);
                response.isSuccess = true;
                response.Message = "Formulario encontrados";
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
            response.Data = await _unitOfWork.Formularios.CountAsync();
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