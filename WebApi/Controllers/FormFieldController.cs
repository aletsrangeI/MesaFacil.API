using Common;
using DTO.FormField;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class FormFieldController : ControllerBase
{
    private readonly IFormFieldApplication _formFieldApplication;

    public FormFieldController(IFormFieldApplication formFieldApplication)
    {
        _formFieldApplication = formFieldApplication;
    }

    #region Metodos sincronos

    [HttpPost("Insert", Name = "FormField_Insert")]
    public ActionResult<Response<bool>> Insert([FromBody] FormFieldDTO dto)
    {
        var response = _formFieldApplication.Insert(dto);
        return Ok(response);
    }

    [HttpPut("Update/{id}", Name = "FormField_Update")]
    public ActionResult<Response<bool>> Update(int id, [FromBody] FormFieldDTO dto)
    {
        try
        {
            dto.Id = id;
        }
        catch
        {
        }
        var response = _formFieldApplication.Update(dto);
        return Ok(response);
    }

    [HttpDelete("Delete/{id}", Name = "FormField_Delete")]
    public ActionResult<Response<bool>> Delete(int id)
    {
        var response = _formFieldApplication.Delete(id);
        return Ok(response);
    }

    [HttpGet("GetById/{id}", Name = "FormField_GetById")]
    public ActionResult<Response<FormFieldDTO>> GetById(int id)
    {
        var response = _formFieldApplication.Get(id);
        if (response is null || response.Data is null)
        {
            var notFound = new Response<FormFieldDTO>
            {
                Data = default!,
                isSuccess = false,
                Message = $"FormField con id {id} no encontrada",
                Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
            };
            return NotFound(notFound);
        }
        return Ok(response);
    }

    [HttpGet("GetAll", Name = "FormField_GetAll")]
    public ActionResult<Response<IEnumerable<FormFieldDTO>>> GetAll()
    {
        var response = _formFieldApplication.GetAll();
        return Ok(response);
    }

    [HttpGet("GetPaged", Name = "FormField_GetPaged")]
    public ActionResult<ResponsePagination<IEnumerable<FormFieldDTO>>> GetPaged(int page, int pageSize)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 10 : pageSize;
        var response = _formFieldApplication.GetAllWithPagination(page, pageSize);
        return Ok(response);
    }

    [HttpGet("Count", Name = "FormField_Count")]
    public ActionResult<Response<int>> Count()
    {
        var response = _formFieldApplication.Count();
        return Ok(response);
    }

    [HttpGet("GetFormFieldByFormCatId/{id}", Name = "FormField_GetFormFieldByFormCatId")]
    public async Task<ActionResult<Response<IEnumerable<FormFieldDTO>>>> GetFormFieldByFormCatId(int id)
    {
        var response = await _formFieldApplication.GetFormFieldByFormIdAsync(id);
        if (response is null || response.Data is null)
        {
            var notFound = new Response<IEnumerable<FormFieldDTO>>
            {
                Data = default!,
                isSuccess = false,
                Message = $"FormField con id {id} no encontrada",
                Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
            };
            return NotFound(notFound);
        }
        return Ok(response);
    }

    #endregion

    #region Metodos asincronos

    [HttpPost("InsertAsync", Name = "FormField_Insert_Async")]
    public async Task<ActionResult<Response<bool>>> InsertAsync([FromBody] FormFieldDTO dto)
    {
        var response = await _formFieldApplication.InsertAsync(dto);
        return Ok(response);
    }

    [HttpPut("UpdateAsync/{id}", Name = "FormField_Update_Async")]
    public async Task<ActionResult<Response<bool>>> UpdateAsync(int id, [FromBody] FormFieldDTO dto)
    {
        try
        {
            dto.Id = id;
        }
        catch
        {
        }
        var response = await _formFieldApplication.UpdateAsync(dto);
        return Ok(response);
    }

    [HttpDelete("DeleteAsync/{id}", Name = "FormField_Delete_Async")]
    public async Task<ActionResult<Response<bool>>> DeleteAsync(int id)
    {
        var response = await _formFieldApplication.DeleteAsync(id);
        return Ok(response);
    }

    [HttpGet("GetByIdAsync/{id}", Name = "FormField_GetById_Async")]
    public async Task<ActionResult<Response<FormFieldDTO>>> GetByIdAsync(int id)
    {
        var response = await _formFieldApplication.GetAsync(id);
        if (response is null || response.Data is null)
        {
            var notFound = new Response<FormFieldDTO>
            {
                Data = default!,
                isSuccess = false,
                Message = $"FormField con id {id} no encontrada",
                Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
            };
            return NotFound(notFound);
        }
        return Ok(response);
    }

    [HttpGet("GetAllAsync", Name = "FormField_GetAll_Async")]
    public async Task<ActionResult<Response<IEnumerable<FormFieldDTO>>>> GetAllAsync()
    {
        var response = await _formFieldApplication.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("GetPagedAsync", Name = "FormField_GetPaged_Async")]
    public async Task<ActionResult<ResponsePagination<IEnumerable<FormFieldDTO>>>> GetPagedAsync(int page, int pageSize)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 10 : pageSize;
        var response = await _formFieldApplication.GetAllWithPaginationAsync(page, pageSize);
        return Ok(response);
    }

    [HttpGet("CountAsync", Name = "FormField_Count_Async")]
    public async Task<ActionResult<Response<int>>> CountAsync()
    {
        var response = await _formFieldApplication.CountAsync();
        return Ok(response);
    }

    [HttpGet("GetFormFieldByFormCatIdAsync/{id}", Name = "FormField_GetFormFieldByFormCatIdAsync_Async")]
    public async Task<ActionResult<Response<IEnumerable<FormFieldDTO>>>> GetFormFieldByFormCatIdAsync(int id)
    {
        var response = await _formFieldApplication.GetFormFieldByFormIdAsync(id);
        if (response is null || response.Data is null)
        {
            var notFound = new Response<IEnumerable<FormFieldDTO>>
            {
                Data = default!,
                isSuccess = false,
                Message = $"FormField con id {id} no encontrada",
                Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
            };
            return NotFound(notFound);
        }
        return Ok(response);
    }

    #endregion
}
