using Common;
using DTO.Turno;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TurnoController : ControllerBase
{
    private readonly ITurnoApplication _turnoApplication;

    public TurnoController(ITurnoApplication turnoApplication)
    {
        _turnoApplication = turnoApplication;
    }

    #region Metodos sincronos

    [HttpPost("Insert")]
    public ActionResult<Response<bool>> Insert([FromBody] TurnoDTO dto)
    {
        var response = _turnoApplication.Insert(dto);
        return Ok(response);
    }

    [HttpGet("GetAll")]
    public ActionResult<Response<IEnumerable<TurnoDTO>>> GetAll()
    {
        var response = _turnoApplication.GetAll();
        return Ok(response);
    }

    [HttpGet("GetById/{id}")]
    public ActionResult<Response<TurnoDTO>> GetById(int id)
    {
        var response = _turnoApplication.Get(id);
        return Ok(response);
    }

    [HttpPut("Update")]
    public ActionResult<Response<bool>> Update([FromBody] TurnoDTO dto)
    {
        var response = _turnoApplication.Update(dto);
        return Ok(response);
    }

    [HttpDelete("Delete/{id}")]
    public ActionResult<Response<bool>> Delete(int id)
    {
        var response = _turnoApplication.Delete(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPagination")]
    public ActionResult<ResponsePagination<IEnumerable<TurnoDTO>>> GetAllWithPagination(int page, int pageSize)
    {
        var response = _turnoApplication.GetAllWithPagination(page, pageSize);
        return Ok(response);
    }

    [HttpGet("Count")]
    public ActionResult<Response<int>> Count()
    {
        var response = _turnoApplication.Count();
        return Ok(response);
    }

    #endregion

    #region Metodos asincronos

    [HttpPost("insert-async")]
    [HttpPost("InsertAsync")]
    public async Task<ActionResult<Response<bool>>> InsertAsync([FromBody] TurnoDTO dto)
    {
        var response = await _turnoApplication.InsertAsync(dto);
        return Ok(response);
    }

    [HttpGet("getall-async")]
    [HttpGet("GetAllAsync")]
    public async Task<ActionResult<Response<IEnumerable<TurnoDTO>>>> GetAllAsync()
    {
        var response = await _turnoApplication.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("getbyid-async/{id}")]
    [HttpGet("GetByIdAsync/{id}")]
    public async Task<ActionResult<Response<TurnoDTO>>> GetByIdAsync(int id)
    {
        var response = await _turnoApplication.GetAsync(id);
        return Ok(response);
    }

    [HttpPut("update-async/{id}")]
    [HttpPut("UpdateAsync")]
    public async Task<ActionResult<Response<bool>>> UpdateAsync([FromBody] TurnoDTO dto, int? id = null)
    {
        if (id.HasValue) dto.Id = id.Value;
        var response = await _turnoApplication.UpdateAsync(dto);
        return Ok(response);
    }

    [HttpDelete("delete-async/{id}")]
    [HttpDelete("DeleteAsync/{id}")]
    public async Task<ActionResult<Response<bool>>> DeleteAsync(int id)
    {
        var response = await _turnoApplication.DeleteAsync(id);
        return Ok(response);
    }

    [HttpGet("getpaged-async")]
    [HttpGet("GetAllWithPaginationAsync")]
    public async Task<ActionResult<ResponsePagination<IEnumerable<TurnoDTO>>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = await _turnoApplication.GetAllWithPaginationAsync(page, pageSize);
        return Ok(response);
    }

    [HttpGet("count-async")]
    [HttpGet("CountAsync")]
    public async Task<ActionResult<Response<int>>> CountAsync()
    {
        var response = await _turnoApplication.CountAsync();
        return Ok(response);
    }

    #endregion
}
