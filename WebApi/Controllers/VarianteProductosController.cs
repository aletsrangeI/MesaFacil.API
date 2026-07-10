using Common;
using DTO.VarianteProducto;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[Route("api/variantes")]
[ApiController]
public class VarianteProductosController : ControllerBase
{
    private readonly IVarianteProductoApplication _varianteProductoApplication;

    public VarianteProductosController(IVarianteProductoApplication varianteProductoApplication)
    {
        _varianteProductoApplication = varianteProductoApplication;
    }

    #region Metodos sincronos

    [HttpPost("Insert")]
    public ActionResult<Response<bool>> Insert([FromBody] VarianteProductoDTO varianteProducto)
    {
        var response = _varianteProductoApplication.Insert(varianteProducto);
        return Ok(response);
    }

    [HttpGet("GetAll")]
    public ActionResult<Response<IEnumerable<VarianteProductoDTO>>> GetAll()
    {
        var response = _varianteProductoApplication.GetAll();
        return Ok(response);
    }

    [HttpGet("GetById/{id}")]
    public ActionResult<Response<VarianteProductoDTO>> GetById(int id)
    {
        var response = _varianteProductoApplication.Get(id);
        return Ok(response);
    }

    [HttpPut("Update")]
    public ActionResult<Response<bool>> Update([FromBody] VarianteProductoDTO varianteProducto)
    {
        var response = _varianteProductoApplication.Update(varianteProducto);
        return Ok(response);
    }

    [HttpDelete("Delete/{id}")]
    public ActionResult<Response<bool>> Delete(int id)
    {
        var response = _varianteProductoApplication.Delete(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPagination")]
    public ActionResult<ResponsePagination<IEnumerable<VarianteProductoDTO>>> GetAllWithPagination(int page, int pageSize)
    {
        var response = _varianteProductoApplication.GetAllWithPagination(page, pageSize);
        return Ok(response);
    }

    [HttpGet("Count")]
    public ActionResult<Response<int>> Count()
    {
        var response = _varianteProductoApplication.Count();
        return Ok(response);
    }

    #endregion

    #region Metodos asincronos

    [HttpPost("InsertAsync")]
    public async Task<ActionResult<Response<bool>>> InsertAsync([FromBody] VarianteProductoDTO varianteProducto)
    {
        var response = await _varianteProductoApplication.InsertAsync(varianteProducto);
        return Ok(response);
    }

    [HttpGet("GetAllAsync")]
    public async Task<ActionResult<Response<IEnumerable<VarianteProductoDTO>>>> GetAllAsync()
    {
        var response = await _varianteProductoApplication.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("GetByIdAsync/{id}")]
    public async Task<ActionResult<Response<VarianteProductoDTO>>> GetByIdAsync(int id)
    {
        var response = await _varianteProductoApplication.GetAsync(id);
        return Ok(response);
    }

    [HttpPut("UpdateAsync")]
    public async Task<ActionResult<Response<bool>>> UpdateAsync([FromBody] VarianteProductoDTO varianteProducto)
    {
        var response = await _varianteProductoApplication.UpdateAsync(varianteProducto);
        return Ok(response);
    }

    [HttpDelete("DeleteAsync/{id}")]
    public async Task<ActionResult<Response<bool>>> DeleteAsync(int id)
    {
        var response = await _varianteProductoApplication.DeleteAsync(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPaginationAsync")]
    public async Task<ActionResult<ResponsePagination<IEnumerable<VarianteProductoDTO>>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = await _varianteProductoApplication.GetAllWithPaginationAsync(page, pageSize);
        return Ok(response);
    }

    [HttpGet("CountAsync")]
    public async Task<ActionResult<Response<int>>> CountAsync()
    {
        var response = await _varianteProductoApplication.CountAsync();
        return Ok(response);
    }

    #endregion
}
