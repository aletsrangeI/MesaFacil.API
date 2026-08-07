using Common;
using DTO.Sucursal;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class SucursalesController : ControllerBase
{
    private readonly ISucursalApplication _sucursalApplication;

    public SucursalesController(ISucursalApplication sucursalApplication)
    {
        _sucursalApplication = sucursalApplication;
    }

    #region Metodos sincronos

    [HttpPost("Insert")]
    public ActionResult<Response<bool>> Insert([FromBody] SucursalDTO sucursal)
    {
        var response = _sucursalApplication.Insert(sucursal);
        return Ok(response);
    }

    [HttpGet("GetAll")]
    public ActionResult<Response<IEnumerable<SucursalDTO>>> GetAll()
    {
        var response = _sucursalApplication.GetAll();
        return Ok(response);
    }

    [HttpGet("GetById/{id}")]
    public ActionResult<Response<SucursalDTO>> GetById(int id)
    {
        var response = _sucursalApplication.Get(id);
        return Ok(response);
    }

    [HttpPut("Update")]
    public ActionResult<Response<bool>> Update([FromBody] SucursalDTO sucursal)
    {
        var response = _sucursalApplication.Update(sucursal);
        return Ok(response);
    }

    [HttpDelete("Delete/{id}")]
    public ActionResult<Response<bool>> Delete(int id)
    {
        var response = _sucursalApplication.Delete(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPagination")]
    public ActionResult<ResponsePagination<IEnumerable<SucursalDTO>>> GetAllWithPagination(int page, int pageSize)
    {
        var response = _sucursalApplication.GetAllWithPagination(page, pageSize);
        return Ok(response);
    }

    [HttpGet("Count")]
    public ActionResult<Response<int>> Count()
    {
        var response = _sucursalApplication.Count();
        return Ok(response);
    }

    #endregion

    #region Metodos asincronos

    [HttpPost("InsertAsync")]
    public async Task<ActionResult<Response<bool>>> InsertAsync([FromBody] SucursalDTO sucursal)
    {
        var response = await _sucursalApplication.InsertAsync(sucursal);
        return Ok(response);
    }

    [HttpGet("GetAllAsync")]
    public async Task<ActionResult<Response<IEnumerable<SucursalDTO>>>> GetAllAsync()
    {
        var response = await _sucursalApplication.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("GetByIdAsync/{id}")]
    public async Task<ActionResult<Response<SucursalDTO>>> GetByIdAsync(int id)
    {
        var response = await _sucursalApplication.GetAsync(id);
        return Ok(response);
    }

    [HttpPut("UpdateAsync")]
    public async Task<ActionResult<Response<bool>>> UpdateAsync([FromBody] SucursalDTO sucursal)
    {
        var response = await _sucursalApplication.UpdateAsync(sucursal);
        return Ok(response);
    }

    [HttpDelete("DeleteAsync/{id}")]
    public async Task<ActionResult<Response<bool>>> DeleteAsync(int id)
    {
        var response = await _sucursalApplication.DeleteAsync(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPaginationAsync")]
    public async Task<ActionResult<ResponsePagination<IEnumerable<SucursalDTO>>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = await _sucursalApplication.GetAllWithPaginationAsync(page, pageSize);
        return Ok(response);
    }

    [HttpGet("CountAsync")]
    public async Task<ActionResult<Response<int>>> CountAsync()
    {
        var response = await _sucursalApplication.CountAsync();
        return Ok(response);
    }

    #endregion
}
