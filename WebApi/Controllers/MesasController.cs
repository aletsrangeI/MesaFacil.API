using Common;
using DTO.Mesa;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class MesasController : ControllerBase
{
    private readonly IMesaApplication _mesaApplication;

    public MesasController(IMesaApplication mesaApplication)
    {
        _mesaApplication = mesaApplication;
    }

    #region Metodos sincronos

    [HttpPost("Insert")]
    public ActionResult<Response<bool>> Insert([FromBody] MesaDTO mesa)
    {
        var response = _mesaApplication.Insert(mesa);
        return Ok(response);
    }

    [HttpGet("GetAll")]
    public ActionResult<Response<IEnumerable<MesaDTO>>> GetAll()
    {
        var response = _mesaApplication.GetAll();
        return Ok(response);
    }

    [HttpGet("GetById/{id}")]
    public ActionResult<Response<MesaDTO>> GetById(int id)
    {
        var response = _mesaApplication.Get(id);
        return Ok(response);
    }

    [HttpPut("Update")]
    public ActionResult<Response<bool>> Update([FromBody] MesaDTO mesa)
    {
        var response = _mesaApplication.Update(mesa);
        return Ok(response);
    }

    [HttpDelete("Delete/{id}")]
    public ActionResult<Response<bool>> Delete(int id)
    {
        var response = _mesaApplication.Delete(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPagination")]
    public ActionResult<ResponsePagination<IEnumerable<MesaDTO>>> GetAllWithPagination(int page, int pageSize)
    {
        var response = _mesaApplication.GetAllWithPagination(page, pageSize);
        return Ok(response);
    }

    [HttpGet("Count")]
    public ActionResult<Response<int>> Count()
    {
        var response = _mesaApplication.Count();
        return Ok(response);
    }

    #endregion

    #region Metodos asincronos

    [HttpPost("InsertAsync")]
    public async Task<ActionResult<Response<bool>>> InsertAsync([FromBody] MesaDTO mesa)
    {
        var response = await _mesaApplication.InsertAsync(mesa);
        return Ok(response);
    }

    [HttpGet("GetAllAsync")]
    public async Task<ActionResult<Response<IEnumerable<MesaDTO>>>> GetAllAsync()
    {
        var response = await _mesaApplication.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("GetByIdAsync/{id}")]
    public async Task<ActionResult<Response<MesaDTO>>> GetByIdAsync(int id)
    {
        var response = await _mesaApplication.GetAsync(id);
        return Ok(response);
    }

    [HttpPut("UpdateAsync")]
    public async Task<ActionResult<Response<bool>>> UpdateAsync([FromBody] MesaDTO mesa)
    {
        var response = await _mesaApplication.UpdateAsync(mesa);
        return Ok(response);
    }

    [HttpDelete("DeleteAsync/{id}")]
    public async Task<ActionResult<Response<bool>>> DeleteAsync(int id)
    {
        var response = await _mesaApplication.DeleteAsync(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPaginationAsync")]
    public async Task<ActionResult<ResponsePagination<IEnumerable<MesaDTO>>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = await _mesaApplication.GetAllWithPaginationAsync(page, pageSize);
        return Ok(response);
    }

    [HttpGet("CountAsync")]
    public async Task<ActionResult<Response<int>>> CountAsync()
    {
        var response = await _mesaApplication.CountAsync();
        return Ok(response);
    }

    #endregion
}
