using Common;
using DTO.GrupoModificador;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[Route("api/grupomodificadores")]
[ApiController]
public class GrupoModificadoresController : ControllerBase
{
    private readonly IGrupoModificadorApplication _grupoModificadorApplication;

    public GrupoModificadoresController(IGrupoModificadorApplication grupoModificadorApplication)
    {
        _grupoModificadorApplication = grupoModificadorApplication;
    }

    #region Metodos sincronos

    [HttpPost("Insert")]
    public ActionResult<Response<bool>> Insert([FromBody] GrupoModificadorDTO grupoModificador)
    {
        var response = _grupoModificadorApplication.Insert(grupoModificador);
        return Ok(response);
    }

    [HttpGet("GetAll")]
    public ActionResult<Response<IEnumerable<GrupoModificadorDTO>>> GetAll()
    {
        var response = _grupoModificadorApplication.GetAll();
        return Ok(response);
    }

    [HttpGet("GetById/{id}")]
    public ActionResult<Response<GrupoModificadorDTO>> GetById(int id)
    {
        var response = _grupoModificadorApplication.Get(id);
        return Ok(response);
    }

    [HttpPut("Update")]
    public ActionResult<Response<bool>> Update([FromBody] GrupoModificadorDTO grupoModificador)
    {
        var response = _grupoModificadorApplication.Update(grupoModificador);
        return Ok(response);
    }

    [HttpDelete("Delete/{id}")]
    public ActionResult<Response<bool>> Delete(int id)
    {
        var response = _grupoModificadorApplication.Delete(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPagination")]
    public ActionResult<ResponsePagination<IEnumerable<GrupoModificadorDTO>>> GetAllWithPagination(int page, int pageSize)
    {
        var response = _grupoModificadorApplication.GetAllWithPagination(page, pageSize);
        return Ok(response);
    }

    [HttpGet("Count")]
    public ActionResult<Response<int>> Count()
    {
        var response = _grupoModificadorApplication.Count();
        return Ok(response);
    }

    #endregion

    #region Metodos asincronos

    [HttpPost("InsertAsync")]
    public async Task<ActionResult<Response<bool>>> InsertAsync([FromBody] GrupoModificadorDTO grupoModificador)
    {
        var response = await _grupoModificadorApplication.InsertAsync(grupoModificador);
        return Ok(response);
    }

    [HttpGet("GetAllAsync")]
    public async Task<ActionResult<Response<IEnumerable<GrupoModificadorDTO>>>> GetAllAsync()
    {
        var response = await _grupoModificadorApplication.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("GetByIdAsync/{id}")]
    public async Task<ActionResult<Response<GrupoModificadorDTO>>> GetByIdAsync(int id)
    {
        var response = await _grupoModificadorApplication.GetAsync(id);
        return Ok(response);
    }

    [HttpPut("UpdateAsync")]
    public async Task<ActionResult<Response<bool>>> UpdateAsync([FromBody] GrupoModificadorDTO grupoModificador)
    {
        var response = await _grupoModificadorApplication.UpdateAsync(grupoModificador);
        return Ok(response);
    }

    [HttpDelete("DeleteAsync/{id}")]
    public async Task<ActionResult<Response<bool>>> DeleteAsync(int id)
    {
        var response = await _grupoModificadorApplication.DeleteAsync(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPaginationAsync")]
    public async Task<ActionResult<ResponsePagination<IEnumerable<GrupoModificadorDTO>>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = await _grupoModificadorApplication.GetAllWithPaginationAsync(page, pageSize);
        return Ok(response);
    }

    [HttpGet("CountAsync")]
    public async Task<ActionResult<Response<int>>> CountAsync()
    {
        var response = await _grupoModificadorApplication.CountAsync();
        return Ok(response);
    }

    #endregion
}
