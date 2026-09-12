using Common;
using DTO.CorteCaja;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CorteCajaController : ControllerBase
{
    private readonly ICorteCajaApplication _corteCajaApplication;

    public CorteCajaController(ICorteCajaApplication corteCajaApplication)
    {
        _corteCajaApplication = corteCajaApplication;
    }

    #region Metodos sincronos

    [HttpPost("Insert")]
    public ActionResult<Response<bool>> Insert([FromBody] CorteCajaDTO dto)
    {
        var response = _corteCajaApplication.Insert(dto);
        return Ok(response);
    }

    [HttpGet("GetAll")]
    public ActionResult<Response<IEnumerable<CorteCajaDTO>>> GetAll()
    {
        var response = _corteCajaApplication.GetAll();
        return Ok(response);
    }

    [HttpGet("GetById/{id}")]
    public ActionResult<Response<CorteCajaDTO>> GetById(int id)
    {
        var response = _corteCajaApplication.Get(id);
        return Ok(response);
    }

    [HttpPut("Update")]
    public ActionResult<Response<bool>> Update([FromBody] CorteCajaDTO dto)
    {
        var response = _corteCajaApplication.Update(dto);
        return Ok(response);
    }

    [HttpDelete("Delete/{id}")]
    public ActionResult<Response<bool>> Delete(int id)
    {
        var response = _corteCajaApplication.Delete(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPagination")]
    public ActionResult<ResponsePagination<IEnumerable<CorteCajaDTO>>> GetAllWithPagination(int page, int pageSize)
    {
        var response = _corteCajaApplication.GetAllWithPagination(page, pageSize);
        return Ok(response);
    }

    [HttpGet("Count")]
    public ActionResult<Response<int>> Count()
    {
        var response = _corteCajaApplication.Count();
        return Ok(response);
    }

    #endregion

    #region Metodos asincronos

    [HttpPost("InsertAsync")]
    public async Task<ActionResult<Response<bool>>> InsertAsync([FromBody] CorteCajaDTO dto)
    {
        var response = await _corteCajaApplication.InsertAsync(dto);
        return Ok(response);
    }

    [HttpGet("GetAllAsync")]
    public async Task<ActionResult<Response<IEnumerable<CorteCajaDTO>>>> GetAllAsync()
    {
        var response = await _corteCajaApplication.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("GetByIdAsync/{id}")]
    public async Task<ActionResult<Response<CorteCajaDTO>>> GetByIdAsync(int id)
    {
        var response = await _corteCajaApplication.GetAsync(id);
        return Ok(response);
    }

    [HttpPut("UpdateAsync")]
    public async Task<ActionResult<Response<bool>>> UpdateAsync([FromBody] CorteCajaDTO dto)
    {
        var response = await _corteCajaApplication.UpdateAsync(dto);
        return Ok(response);
    }

    [HttpDelete("DeleteAsync/{id}")]
    public async Task<ActionResult<Response<bool>>> DeleteAsync(int id)
    {
        var response = await _corteCajaApplication.DeleteAsync(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPaginationAsync")]
    public async Task<ActionResult<ResponsePagination<IEnumerable<CorteCajaDTO>>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = await _corteCajaApplication.GetAllWithPaginationAsync(page, pageSize);
        return Ok(response);
    }

    [HttpGet("CountAsync")]
    public async Task<ActionResult<Response<int>>> CountAsync()
    {
        var response = await _corteCajaApplication.CountAsync();
        return Ok(response);
    }

    [HttpGet("resumen-actual")]
    public async Task<ActionResult<Response<ResumenCorteDTO>>> ObtenerResumenActualAsync([FromQuery] int? idSucursal, [FromQuery] int? idTurno)
    {
        var response = await _corteCajaApplication.ObtenerResumenActualAsync(idSucursal, idTurno);
        return Ok(response);
    }

    [HttpPost("realizar-corte")]
    public async Task<ActionResult<Response<CorteCajaDTO>>> RealizarCorteAsync([FromBody] RealizarCorteRequestDTO dto)
    {
        var claim = User.FindFirst("idUsuario") ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        int? userId = claim != null && int.TryParse(claim.Value, out var id) ? id : null;

        var response = await _corteCajaApplication.RealizarCorteAsync(dto, userId);
        return Ok(response);
    }

    [HttpGet("historial")]
    public async Task<ActionResult<Response<ResumenHistorialCortesDTO>>> ObtenerHistorialAsync([FromQuery] DateTime? fechaInicio, [FromQuery] DateTime? fechaFin, [FromQuery] int? idSucursal)
    {
        var response = await _corteCajaApplication.ObtenerHistorialAsync(fechaInicio, fechaFin, idSucursal);
        return Ok(response);
    }

    #endregion
}
