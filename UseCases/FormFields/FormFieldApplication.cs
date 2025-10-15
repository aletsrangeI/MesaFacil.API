using AutoMapper;
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
    private readonly IMapper _mapper;
    private readonly FormFieldDTOValidator _validationRules;
    private readonly IAppLogger<FormFieldApplication> _logger;

    public FormFieldApplication(
        IUnitOfWork unitOfWork,
        IMapper mapper,
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

    public ResponsePagination<IEnumerable<FormFieldDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<FormFieldDTO>>();
        try
        {
            var list = _unitOfWork.FormFields.GetAllWithPagination(page, pageSize);

            if (list != null)
            {
                response.Data = _mapper.Map<IEnumerable<FormFieldDTO>>(list);
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

    public Response<IEnumerable<FormFieldDTO>> GetFormFieldByFormCatId(int id)
    {
        var response = new Response<IEnumerable<FormFieldDTO>>();
        try
        {
            var list = _unitOfWork.FormFields.GetFormFieldByFormCatId(id);
            response.Data = _mapper.Map<IEnumerable<FormFieldDTO>>(list);
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

    #endregion

    #region Metodos asincronos

    public async Task<Response<bool>> InsertAsync(FormFieldDTO dto)
    {
        var response = new Response<bool>();
        try
        {
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

    public async Task<ResponsePagination<IEnumerable<FormFieldDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<FormFieldDTO>>();
        try
        {
            var list = await _unitOfWork.FormFields.GetAllWithPaginationAsync(page, pageSize);

            if (list != null)
            {
                response.Data = _mapper.Map<IEnumerable<FormFieldDTO>>(list);
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

    public async Task<Response<int>> CountAsync()
    {
        var response = new Response<int>();
        try
        {
            response.Data = await _unitOfWork.FormFields.CountAsync();

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

    public async Task<Response<IEnumerable<FormFieldDTO>>> GetFormFieldByFormCatIdAsync(int id)
    {
        var response = new Response<IEnumerable<FormFieldDTO>>();
        try
        {
            var list = await _unitOfWork.FormFields.GetFormFieldByFormCatIdAsync(id);
            response.Data = _mapper.Map<IEnumerable<FormFieldDTO>>(list);

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

    #endregion
}