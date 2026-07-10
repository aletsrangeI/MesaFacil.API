using Common;
using DTO.CategoriaMenu;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[Route("api/categorias")]
[ApiController]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaMenuApplication _categoriaApplication;

    public CategoriasController(ICategoriaMenuApplication categoriaApplication)
    {
        _categoriaApplication = categoriaApplication;
    }

    #region Metodos sincronos

    [HttpPost("Insert")]
    public ActionResult<Response<bool>> Insert([FromBody] CategoriaMenuDTO categoria)
    {
        var response = _categoriaApplication.Insert(categoria);
        return Ok(response);
    }

    [HttpGet("GetAll")]
    public ActionResult<Response<IEnumerable<CategoriaMenuDTO>>> GetAll()
    {
        var response = _categoriaApplication.GetAll();
        return Ok(response);
    }

    [HttpGet("GetById/{id}")]
    public ActionResult<Response<CategoriaMenuDTO>> GetById(int id)
    {
        var response = _categoriaApplication.Get(id);
        return Ok(response);
    }

    [HttpPut("Update")]
    public ActionResult<Response<bool>> Update([FromBody] CategoriaMenuDTO categoria)
    {
        var response = _categoriaApplication.Update(categoria);
        return Ok(response);
    }

    [HttpDelete("Delete/{id}")]
    public ActionResult<Response<bool>> Delete(int id)
    {
        var response = _categoriaApplication.Delete(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPagination")]
    public ActionResult<ResponsePagination<IEnumerable<CategoriaMenuDTO>>> GetAllWithPagination(int page, int pageSize)
    {
        var response = _categoriaApplication.GetAllWithPagination(page, pageSize);
        return Ok(response);
    }

    [HttpGet("Count")]
    public ActionResult<Response<int>> Count()
    {
        var response = _categoriaApplication.Count();
        return Ok(response);
    }

    #endregion

    #region Metodos asincronos

    [HttpPost("InsertAsync")]
    public async Task<ActionResult<Response<bool>>> InsertAsync([FromBody] CategoriaMenuDTO categoria)
    {
        var response = await _categoriaApplication.InsertAsync(categoria);
        return Ok(response);
    }

    [HttpGet("GetAllAsync")]
    public async Task<ActionResult<Response<IEnumerable<CategoriaMenuDTO>>>> GetAllAsync()
    {
        var response = await _categoriaApplication.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("GetByIdAsync/{id}")]
    public async Task<ActionResult<Response<CategoriaMenuDTO>>> GetByIdAsync(int id)
    {
        var response = await _categoriaApplication.GetAsync(id);
        return Ok(response);
    }

    [HttpPut("UpdateAsync")]
    public async Task<ActionResult<Response<bool>>> UpdateAsync([FromBody] CategoriaMenuDTO categoria)
    {
        var response = await _categoriaApplication.UpdateAsync(categoria);
        return Ok(response);
    }

    [HttpDelete("DeleteAsync/{id}")]
    public async Task<ActionResult<Response<bool>>> DeleteAsync(int id)
    {
        var response = await _categoriaApplication.DeleteAsync(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPaginationAsync")]
    public async Task<ActionResult<ResponsePagination<IEnumerable<CategoriaMenuDTO>>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = await _categoriaApplication.GetAllWithPaginationAsync(page, pageSize);
        return Ok(response);
    }

    [HttpGet("CountAsync")]
    public async Task<ActionResult<Response<int>>> CountAsync()
    {
        var response = await _categoriaApplication.CountAsync();
        return Ok(response);
    }

    #endregion
}
