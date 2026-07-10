using Common;
using DTO.Producto;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[Route("api/productos")]
[ApiController]
public class ProductosController : ControllerBase
{
    private readonly IProductoApplication _productoApplication;

    public ProductosController(IProductoApplication productoApplication)
    {
        _productoApplication = productoApplication;
    }

    #region Metodos sincronos

    [HttpPost("Insert")]
    public ActionResult<Response<bool>> Insert([FromBody] ProductoDTO producto)
    {
        var response = _productoApplication.Insert(producto);
        return Ok(response);
    }

    [HttpGet("GetAll")]
    public ActionResult<Response<IEnumerable<ProductoDTO>>> GetAll()
    {
        var response = _productoApplication.GetAll();
        return Ok(response);
    }

    [HttpGet("GetById/{id}")]
    public ActionResult<Response<ProductoDTO>> GetById(int id)
    {
        var response = _productoApplication.Get(id);
        return Ok(response);
    }

    [HttpPut("Update")]
    public ActionResult<Response<bool>> Update([FromBody] ProductoDTO producto)
    {
        var response = _productoApplication.Update(producto);
        return Ok(response);
    }

    [HttpDelete("Delete/{id}")]
    public ActionResult<Response<bool>> Delete(int id)
    {
        var response = _productoApplication.Delete(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPagination")]
    public ActionResult<ResponsePagination<IEnumerable<ProductoDTO>>> GetAllWithPagination(int page, int pageSize)
    {
        var response = _productoApplication.GetAllWithPagination(page, pageSize);
        return Ok(response);
    }

    [HttpGet("Count")]
    public ActionResult<Response<int>> Count()
    {
        var response = _productoApplication.Count();
        return Ok(response);
    }

    #endregion

    #region Metodos asincronos

    [HttpPost("InsertAsync")]
    public async Task<ActionResult<Response<bool>>> InsertAsync([FromBody] ProductoDTO producto)
    {
        var response = await _productoApplication.InsertAsync(producto);
        return Ok(response);
    }

    [HttpGet("GetAllAsync")]
    public async Task<ActionResult<Response<IEnumerable<ProductoDTO>>>> GetAllAsync()
    {
        var response = await _productoApplication.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("GetByIdAsync/{id}")]
    public async Task<ActionResult<Response<ProductoDTO>>> GetByIdAsync(int id)
    {
        var response = await _productoApplication.GetAsync(id);
        return Ok(response);
    }

    [HttpPut("UpdateAsync")]
    public async Task<ActionResult<Response<bool>>> UpdateAsync([FromBody] ProductoDTO producto)
    {
        var response = await _productoApplication.UpdateAsync(producto);
        return Ok(response);
    }

    [HttpDelete("DeleteAsync/{id}")]
    public async Task<ActionResult<Response<bool>>> DeleteAsync(int id)
    {
        var response = await _productoApplication.DeleteAsync(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPaginationAsync")]
    public async Task<ActionResult<ResponsePagination<IEnumerable<ProductoDTO>>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = await _productoApplication.GetAllWithPaginationAsync(page, pageSize);
        return Ok(response);
    }

    [HttpGet("CountAsync")]
    public async Task<ActionResult<Response<int>>> CountAsync()
    {
        var response = await _productoApplication.CountAsync();
        return Ok(response);
    }

    #endregion
}
