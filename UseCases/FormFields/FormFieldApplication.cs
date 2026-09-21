using Interface.Mapping;
using Common;
using Domain.Entities;
using DTO.FormField;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.FormFields;

public class FormFieldApplication : IFormFieldApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppMapper _mapper;
    private readonly FormFieldDTOValidator _validationRules;
    private readonly IAppLogger<FormFieldApplication> _logger;

    public FormFieldApplication(
        IUnitOfWork unitOfWork,
        IAppMapper mapper,
        FormFieldDTOValidator validationRules,
        IAppLogger<FormFieldApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(FormFieldDTO dto)
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

            var entity = _mapper.Map<FormField>(dto);
            response.Data = _unitOfWork.FormFields.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "FormField creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<bool> Update(FormFieldDTO dto)
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

            var entity = _mapper.Map<FormField>(dto);
            response.Data = _unitOfWork.FormFields.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "FormField modificado correctamente";
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
            response.Data = _unitOfWork.FormFields.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "FormField eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<FormFieldDTO> Get(int id)
    {
        var response = new Response<FormFieldDTO>();
        try
        {
            var entity = _unitOfWork.FormFields.Get(id);
            response.Data = _mapper.Map<FormFieldDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "FormField encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<IEnumerable<FormFieldDTO>> GetAll()
    {
        var response = new Response<IEnumerable<FormFieldDTO>>();
        try
        {
            var list = _unitOfWork.FormFields.GetAll();
            response.Data = _mapper.Map<IEnumerable<FormFieldDTO>>(list);
            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "FormFields encontrados";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public ResponsePagination<IEnumerable<FormFieldDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<FormFieldDTO>>();
        try
        {
            var list = _unitOfWork.FormFields.GetAllWithPagination(page, pageSize);
            var count = _unitOfWork.FormFields.Count();

            if (list != null)
            {
                response.Data = _mapper.Map<IEnumerable<FormFieldDTO>>(list);
                response.PageNumber = page;
                response.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                response.TotalCount = count;
                response.isSuccess = true;
                response.Message = "Consulta paginada exitosa";
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
            response.Data = _unitOfWork.FormFields.Count();
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<IEnumerable<FormFieldDTO>> GetFormFieldByFormCode(string code)
    {
        var response = new Response<IEnumerable<FormFieldDTO>>();
        try
        {
            var list = _unitOfWork.FormFields.GetFormFieldByFormCode(code);
            response.Data = _mapper.Map<IEnumerable<FormFieldDTO>>(list);
            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Campos del formulario obtenidos correctamente";
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

    #region Metodos asincronos

    public async Task<Response<bool>> InsertAsync(FormFieldDTO dto)
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

            var entity = _mapper.Map<FormField>(dto);
            response.Data = await _unitOfWork.FormFields.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "FormField creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<bool>> UpdateAsync(FormFieldDTO dto)
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

            var entity = _mapper.Map<FormField>(dto);
            response.Data = await _unitOfWork.FormFields.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "FormField modificado correctamente";
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
            response.Data = await _unitOfWork.FormFields.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "FormField eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<FormFieldDTO>> GetAsync(int id)
    {
        var response = new Response<FormFieldDTO>();
        try
        {
            var entity = await _unitOfWork.FormFields.GetAsync(id);
            response.Data = _mapper.Map<FormFieldDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "FormField encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<IEnumerable<FormFieldDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<FormFieldDTO>>();
        try
        {
            var list = await _unitOfWork.FormFields.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<FormFieldDTO>>(list);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "FormFields encontrados";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<ResponsePagination<IEnumerable<FormFieldDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<FormFieldDTO>>();
        try
        {
            var list = await _unitOfWork.FormFields.GetAllWithPaginationAsync(page, pageSize);
            var count = await _unitOfWork.FormFields.CountAsync();

            if (list != null)
            {
                response.Data = _mapper.Map<IEnumerable<FormFieldDTO>>(list);
                response.PageNumber = page;
                response.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                response.TotalCount = count;
                response.isSuccess = true;
                response.Message = "Consulta paginada exitosa";
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
            response.Data = await _unitOfWork.FormFields.CountAsync();
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<IEnumerable<FormFieldDTO>>> GetFormFieldByFormIdAsync(int formularioId)
    {
        var response = new Response<IEnumerable<FormFieldDTO>>();
        try
        {
            var list = await _unitOfWork.FormFields.GetFormFieldByFormIdAsync(formularioId);
            response.Data = _mapper.Map<IEnumerable<FormFieldDTO>>(list);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Campos del formulario obtenidos correctamente";
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