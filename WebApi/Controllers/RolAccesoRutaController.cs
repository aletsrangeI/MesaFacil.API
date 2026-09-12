using Common;
using DTO.RolAccesoRuta;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class RolAccesoRutaController : ControllerBase
{
    private readonly IRolAccesoRutaApplication _application;

    public RolAccesoRutaController(IRolAccesoRutaApplication application)
    {
        _application = application;
    }

    #region Metodos sincronos

    [HttpPost("Insert")]
    public ActionResult<Response<bool>> Insert([FromBody] RolAccesoRutaDTO dto)
    {
        var response = _application.Insert(dto);
        return Ok(response);
    }

    [HttpGet("GetAll")]
    public ActionResult<Response<IEnumerable<RolAccesoRutaDTO>>> GetAll()
    {
        var response = _application.GetAll();
        return Ok(response);
    }

    [HttpGet("GetById/{id}")]
    public ActionResult<Response<RolAccesoRutaDTO>> GetById(int id)
    {
        var response = _application.Get(id);
        return Ok(response);
    }

    [HttpPut("Update")]
    public ActionResult<Response<bool>> Update([FromBody] RolAccesoRutaDTO dto)
    {
        var response = _application.Update(dto);
        return Ok(response);
    }

    [HttpDelete("Delete/{id}")]
    public ActionResult<Response<bool>> Delete(int id)
    {
        var response = _application.Delete(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPagination")]
    public ActionResult<ResponsePagination<IEnumerable<RolAccesoRutaDTO>>> GetAllWithPagination(int page, int pageSize)
    {
        var response = _application.GetAllWithPagination(page, pageSize);
        return Ok(response);
    }

    [HttpGet("Count")]
    public ActionResult<Response<int>> Count()
    {
        var response = _application.Count();
        return Ok(response);
    }

    #endregion

    #region Metodos asincronos

    [HttpPost("InsertAsync")]
    public async Task<ActionResult<Response<bool>>> InsertAsync([FromBody] RolAccesoRutaDTO dto)
    {
        var response = await _application.InsertAsync(dto);
        return Ok(response);
    }

    [HttpGet("GetAllAsync")]
    public async Task<ActionResult<Response<IEnumerable<RolAccesoRutaDTO>>>> GetAllAsync()
    {
        var response = await _application.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("GetByIdAsync/{id}")]
    public async Task<ActionResult<Response<RolAccesoRutaDTO>>> GetByIdAsync(int id)
    {
        var response = await _application.GetAsync(id);
        return Ok(response);
    }

    [HttpPut("UpdateAsync")]
    public async Task<ActionResult<Response<bool>>> UpdateAsync([FromBody] RolAccesoRutaDTO dto)
    {
        var response = await _application.UpdateAsync(dto);
        return Ok(response);
    }

    [HttpDelete("DeleteAsync/{id}")]
    public async Task<ActionResult<Response<bool>>> DeleteAsync(int id)
    {
        var response = await _application.DeleteAsync(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPaginationAsync")]
    public async Task<ActionResult<ResponsePagination<IEnumerable<RolAccesoRutaDTO>>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = await _application.GetAllWithPaginationAsync(page, pageSize);
        return Ok(response);
    }

    [HttpGet("CountAsync")]
    public async Task<ActionResult<Response<int>>> CountAsync()
    {
        var response = await _application.CountAsync();
        return Ok(response);
    }

    #endregion
}
