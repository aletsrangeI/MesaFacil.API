using Common;
using DTO.Formulario;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class FormularioController : ControllerBase
{
    private readonly IFormularioApplication _formularioApplication;

    public FormularioController(IFormularioApplication formularioApplication)
    {
        _formularioApplication = formularioApplication;
    }

    #region Metodos sincronos

    [HttpPost("Insert", Name = "Formulario_Insert")]
    public ActionResult<Response<bool>> Insert([FromBody] FormularioDTO dto)
    {
        var response = _formularioApplication.Insert(dto);
        return Ok(response);
    }

    [HttpPut("Update/{id}", Name = "Formulario_Update")]
    public ActionResult<Response<bool>> Update(int id, [FromBody] FormularioDTO dto)
    {
        try
        {
            dto.Id = id;
        }
        catch
        {
        }
        var response = _formularioApplication.Update(dto);
        return Ok(response);
    }

    [HttpDelete("Delete/{id}", Name = "Formulario_Delete")]
    public ActionResult<Response<bool>> Delete(int id)
    {
        var response = _formularioApplication.Delete(id);
        return Ok(response);
    }

    [HttpGet("GetById/{id}", Name = "Formulario_GetById")]
    public ActionResult<Response<FormularioDTO>> GetById(int id)
    {
        var response = _formularioApplication.Get(id);
        if (response is null || response.Data is null)
        {
            var notFound = new Response<FormularioDTO>
            {
                Data = default!,
                isSuccess = false,
                Message = $"Formulario con id {id} no encontrada",
                Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
            };
            return NotFound(notFound);
        }
        return Ok(response);
    }

    [HttpGet("GetAll", Name = "Formulario_GetAll")]
    public ActionResult<Response<IEnumerable<FormularioDTO>>> GetAll()
    {
        var response = _formularioApplication.GetAll();
        return Ok(response);
    }

    [HttpGet("GetPaged", Name = "Formulario_GetPaged")]
    public ActionResult<ResponsePagination<IEnumerable<FormularioDTO>>> GetPaged(int page, int pageSize)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 10 : pageSize;
        var response = _formularioApplication.GetAllWithPagination(page, pageSize);
        return Ok(response);
    }

    [HttpGet("Count", Name = "Formulario_Count")]
    public ActionResult<Response<int>> Count()
    {
        var response = _formularioApplication.Count();
        return Ok(response);
    }

    #endregion

    #region Metodos asincronos

    [HttpPost("InsertAsync", Name = "Formulario_Insert_Async")]
    public async Task<ActionResult<Response<bool>>> InsertAsync([FromBody] FormularioDTO dto)
    {
        var response = await _formularioApplication.InsertAsync(dto);
        return Ok(response);
    }

    [HttpPut("UpdateAsync/{id}", Name = "Formulario_Update_Async")]
    public async Task<ActionResult<Response<bool>>> UpdateAsync(int id, [FromBody] FormularioDTO dto)
    {
        try
        {
            dto.Id = id;
        }
        catch
        {
        }
        var response = await _formularioApplication.UpdateAsync(dto);
        return Ok(response);
    }

    [HttpDelete("DeleteAsync/{id}", Name = "Formulario_Delete_Async")]
    public async Task<ActionResult<Response<bool>>> DeleteAsync(int id)
    {
        var response = await _formularioApplication.DeleteAsync(id);
        return Ok(response);
    }

    [HttpGet("GetByIdAsync/{id}", Name = "Formulario_GetById_Async")]
    public async Task<ActionResult<Response<FormularioDTO>>> GetByIdAsync(int id)
    {
        var response = await _formularioApplication.GetAsync(id);
        if (response is null || response.Data is null)
        {
            var notFound = new Response<FormularioDTO>
            {
                Data = default!,
                isSuccess = false,
                Message = $"Formulario con id {id} no encontrada",
                Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
            };
            return NotFound(notFound);
        }
        return Ok(response);
    }

    [HttpGet("GetAllAsync", Name = "Formulario_GetAll_Async")]
    public async Task<ActionResult<Response<IEnumerable<FormularioDTO>>>> GetAllAsync()
    {
        var response = await _formularioApplication.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("GetPagedAsync", Name = "Formulario_GetPaged_Async")]
    public async Task<ActionResult<ResponsePagination<IEnumerable<FormularioDTO>>>> GetPagedAsync(int page, int pageSize)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 10 : pageSize;
        var response = await _formularioApplication.GetAllWithPaginationAsync(page, pageSize);
        return Ok(response);
    }

    [HttpGet("CountAsync", Name = "Formulario_Count_Async")]
    public async Task<ActionResult<Response<int>>> CountAsync()
    {
        var response = await _formularioApplication.CountAsync();
        return Ok(response);
    }

    #endregion
}
