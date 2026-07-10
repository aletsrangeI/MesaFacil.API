using Common;
using DTO.OpcionModificador;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[Route("api/opcionmodificadores")]
[ApiController]
public class OpcionModificadoresController : ControllerBase
{
    private readonly IOpcionModificadorApplication _opcionModificadorApplication;

    public OpcionModificadoresController(IOpcionModificadorApplication opcionModificadorApplication)
    {
        _opcionModificadorApplication = opcionModificadorApplication;
    }

    #region Metodos sincronos

    [HttpPost("Insert")]
    public ActionResult<Response<bool>> Insert([FromBody] OpcionModificadorDTO opcionModificador)
    {
        var response = _opcionModificadorApplication.Insert(opcionModificador);
        return Ok(response);
    }

    [HttpGet("GetAll")]
    public ActionResult<Response<IEnumerable<OpcionModificadorDTO>>> GetAll()
    {
        var response = _opcionModificadorApplication.GetAll();
        return Ok(response);
    }

    [HttpGet("GetById/{id}")]
    public ActionResult<Response<OpcionModificadorDTO>> GetById(int id)
    {
        var response = _opcionModificadorApplication.Get(id);
        return Ok(response);
    }

    [HttpPut("Update")]
    public ActionResult<Response<bool>> Update([FromBody] OpcionModificadorDTO opcionModificador)
    {
        var response = _opcionModificadorApplication.Update(opcionModificador);
        return Ok(response);
    }

    [HttpDelete("Delete/{id}")]
    public ActionResult<Response<bool>> Delete(int id)
    {
        var response = _opcionModificadorApplication.Delete(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPagination")]
    public ActionResult<ResponsePagination<IEnumerable<OpcionModificadorDTO>>> GetAllWithPagination(int page, int pageSize)
    {
        var response = _opcionModificadorApplication.GetAllWithPagination(page, pageSize);
        return Ok(response);
    }

    [HttpGet("Count")]
    public ActionResult<Response<int>> Count()
    {
        var response = _opcionModificadorApplication.Count();
        return Ok(response);
    }

    #endregion

    #region Metodos asincronos

    [HttpPost("InsertAsync")]
    public async Task<ActionResult<Response<bool>>> InsertAsync([FromBody] OpcionModificadorDTO opcionModificador)
    {
        var response = await _opcionModificadorApplication.InsertAsync(opcionModificador);
        return Ok(response);
    }

    [HttpGet("GetAllAsync")]
    public async Task<ActionResult<Response<IEnumerable<OpcionModificadorDTO>>>> GetAllAsync()
    {
        var response = await _opcionModificadorApplication.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("GetByIdAsync/{id}")]
    public async Task<ActionResult<Response<OpcionModificadorDTO>>> GetByIdAsync(int id)
    {
        var response = await _opcionModificadorApplication.GetAsync(id);
        return Ok(response);
    }

    [HttpPut("UpdateAsync")]
    public async Task<ActionResult<Response<bool>>> UpdateAsync([FromBody] OpcionModificadorDTO opcionModificador)
    {
        var response = await _opcionModificadorApplication.UpdateAsync(opcionModificador);
        return Ok(response);
    }

    [HttpDelete("DeleteAsync/{id}")]
    public async Task<ActionResult<Response<bool>>> DeleteAsync(int id)
    {
        var response = await _opcionModificadorApplication.DeleteAsync(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPaginationAsync")]
    public async Task<ActionResult<ResponsePagination<IEnumerable<OpcionModificadorDTO>>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = await _opcionModificadorApplication.GetAllWithPaginationAsync(page, pageSize);
        return Ok(response);
    }

    [HttpGet("CountAsync")]
    public async Task<ActionResult<Response<int>>> CountAsync()
    {
        var response = await _opcionModificadorApplication.CountAsync();
        return Ok(response);
    }

    #endregion
}
