using Common;
using DTO.Empresa;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class EmpresaController : ControllerBase
{
    private readonly IEmpresaApplication _empresaApplication;

    public EmpresaController(IEmpresaApplication empresaApplication)
    {
        _empresaApplication = empresaApplication;
    }

    #region Metodos sincronos

    [HttpPost("Insert")]
    public ActionResult<Response<bool>> Insert([FromBody] EmpresaDTO empresa)
    {
        var response = _empresaApplication.Insert(empresa);
        return Ok(response);
    }

    [HttpGet("GetAll")]
    public ActionResult<Response<IEnumerable<EmpresaDTO>>> GetAll()
    {
        var response = _empresaApplication.GetAll();
        return Ok(response);
    }

    [HttpGet("GetById/{id}")]
    public ActionResult<Response<EmpresaDTO>> GetById(int id)
    {
        var response = _empresaApplication.Get(id);
        return Ok(response);
    }

    [HttpPut("Update")]
    public ActionResult<Response<bool>> Update([FromBody] EmpresaDTO empresa)
    {
        var response = _empresaApplication.Update(empresa);
        return Ok(response);
    }

    [HttpDelete("Delete/{id}")]
    public ActionResult<Response<bool>> Delete(int id)
    {
        var response = _empresaApplication.Delete(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPagination")]
    public ActionResult<ResponsePagination<IEnumerable<EmpresaDTO>>> GetAllWithPagination(int page, int pageSize)
    {
        var response = _empresaApplication.GetAllWithPagination(page, pageSize);
        return Ok(response);
    }

    [HttpGet("Count")]
    public ActionResult<Response<int>> Count()
    {
        var response = _empresaApplication.Count();
        return Ok(response);
    }

    #endregion

    #region Metodos asincronos

    [HttpPost("InsertAsync")]
    public async Task<ActionResult<Response<bool>>> InsertAsync([FromBody] EmpresaDTO empresa)
    {
        var response = await _empresaApplication.InsertAsync(empresa);
        return Ok(response);
    }

    [HttpGet("GetAllAsync")]
    public async Task<ActionResult<Response<IEnumerable<EmpresaDTO>>>> GetAllAsync()
    {
        var response = await _empresaApplication.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("GetByIdAsync/{id}")]
    public async Task<ActionResult<Response<EmpresaDTO>>> GetByIdAsync(int id)
    {
        var response = await _empresaApplication.GetAsync(id);
        return Ok(response);
    }

    [HttpPut("UpdateAsync")]
    public async Task<ActionResult<Response<bool>>> UpdateAsync([FromBody] EmpresaDTO empresa)
    {
        var response = await _empresaApplication.UpdateAsync(empresa);
        return Ok(response);
    }

    [HttpDelete("DeleteAsync/{id}")]
    public async Task<ActionResult<Response<bool>>> DeleteAsync(int id)
    {
        var response = await _empresaApplication.DeleteAsync(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPaginationAsync")]
    public async Task<ActionResult<ResponsePagination<IEnumerable<EmpresaDTO>>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = await _empresaApplication.GetAllWithPaginationAsync(page, pageSize);
        return Ok(response);
    }

    [HttpGet("CountAsync")]
    public async Task<ActionResult<Response<int>>> CountAsync()
    {
        var response = await _empresaApplication.CountAsync();
        return Ok(response);
    }

    #endregion
}
