using Common;
using DTO.Precio;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[Route("api/precios")]
[ApiController]
public class PreciosController : ControllerBase
{
    private readonly IPrecioApplication _precioApplication;

    public PreciosController(IPrecioApplication precioApplication)
    {
        _precioApplication = precioApplication;
    }

    #region Metodos sincronos

    [HttpPost("Insert")]
    public ActionResult<Response<bool>> Insert([FromBody] PrecioDTO precio)
    {
        var response = _precioApplication.Insert(precio);
        return Ok(response);
    }

    [HttpGet("GetAll")]
    public ActionResult<Response<IEnumerable<PrecioDTO>>> GetAll()
    {
        var response = _precioApplication.GetAll();
        return Ok(response);
    }

    [HttpGet("GetById/{id}")]
    public ActionResult<Response<PrecioDTO>> GetById(int id)
    {
        var response = _precioApplication.Get(id);
        return Ok(response);
    }

    [HttpPut("Update")]
    public ActionResult<Response<bool>> Update([FromBody] PrecioDTO precio)
    {
        var response = _precioApplication.Update(precio);
        return Ok(response);
    }

    [HttpDelete("Delete/{id}")]
    public ActionResult<Response<bool>> Delete(int id)
    {
        var response = _precioApplication.Delete(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPagination")]
    public ActionResult<ResponsePagination<IEnumerable<PrecioDTO>>> GetAllWithPagination(int page, int pageSize)
    {
        var response = _precioApplication.GetAllWithPagination(page, pageSize);
        return Ok(response);
    }

    [HttpGet("Count")]
    public ActionResult<Response<int>> Count()
    {
        var response = _precioApplication.Count();
        return Ok(response);
    }

    #endregion

    #region Metodos asincronos

    [HttpPost("InsertAsync")]
    public async Task<ActionResult<Response<bool>>> InsertAsync([FromBody] PrecioDTO precio)
    {
        var response = await _precioApplication.InsertAsync(precio);
        return Ok(response);
    }

    [HttpGet("GetAllAsync")]
    public async Task<ActionResult<Response<IEnumerable<PrecioDTO>>>> GetAllAsync()
    {
        var response = await _precioApplication.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("GetByIdAsync/{id}")]
    public async Task<ActionResult<Response<PrecioDTO>>> GetByIdAsync(int id)
    {
        var response = await _precioApplication.GetAsync(id);
        return Ok(response);
    }

    [HttpPut("UpdateAsync")]
    public async Task<ActionResult<Response<bool>>> UpdateAsync([FromBody] PrecioDTO precio)
    {
        var response = await _precioApplication.UpdateAsync(precio);
        return Ok(response);
    }

    [HttpDelete("DeleteAsync/{id}")]
    public async Task<ActionResult<Response<bool>>> DeleteAsync(int id)
    {
        var response = await _precioApplication.DeleteAsync(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPaginationAsync")]
    public async Task<ActionResult<ResponsePagination<IEnumerable<PrecioDTO>>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = await _precioApplication.GetAllWithPaginationAsync(page, pageSize);
        return Ok(response);
    }

    [HttpGet("CountAsync")]
    public async Task<ActionResult<Response<int>>> CountAsync()
    {
        var response = await _precioApplication.CountAsync();
        return Ok(response);
    }

    #endregion
}
