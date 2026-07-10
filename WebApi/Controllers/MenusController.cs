using Common;
using DTO.Menu;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[Route("api/menus")]
[ApiController]
public class MenusController : ControllerBase
{
    private readonly IMenuApplication _menuApplication;

    public MenusController(IMenuApplication menuApplication)
    {
        _menuApplication = menuApplication;
    }

    #region Metodos sincronos

    [HttpPost("Insert")]
    public ActionResult<Response<bool>> Insert([FromBody] MenuDTO menu)
    {
        var response = _menuApplication.Insert(menu);
        return Ok(response);
    }

    [HttpGet("GetAll")]
    public ActionResult<Response<IEnumerable<MenuDTO>>> GetAll()
    {
        var response = _menuApplication.GetAll();
        return Ok(response);
    }

    [HttpGet("GetById/{id}")]
    public ActionResult<Response<MenuDTO>> GetById(int id)
    {
        var response = _menuApplication.Get(id);
        return Ok(response);
    }

    [HttpPut("Update")]
    public ActionResult<Response<bool>> Update([FromBody] MenuDTO menu)
    {
        var response = _menuApplication.Update(menu);
        return Ok(response);
    }

    [HttpDelete("Delete/{id}")]
    public ActionResult<Response<bool>> Delete(int id)
    {
        var response = _menuApplication.Delete(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPagination")]
    public ActionResult<ResponsePagination<IEnumerable<MenuDTO>>> GetAllWithPagination(int page, int pageSize)
    {
        var response = _menuApplication.GetAllWithPagination(page, pageSize);
        return Ok(response);
    }

    [HttpGet("Count")]
    public ActionResult<Response<int>> Count()
    {
        var response = _menuApplication.Count();
        return Ok(response);
    }

    #endregion

    #region Metodos asincronos

    [HttpPost("InsertAsync")]
    public async Task<ActionResult<Response<bool>>> InsertAsync([FromBody] MenuDTO menu)
    {
        var response = await _menuApplication.InsertAsync(menu);
        return Ok(response);
    }

    [HttpGet("GetAllAsync")]
    public async Task<ActionResult<Response<IEnumerable<MenuDTO>>>> GetAllAsync()
    {
        var response = await _menuApplication.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("GetByIdAsync/{id}")]
    public async Task<ActionResult<Response<MenuDTO>>> GetByIdAsync(int id)
    {
        var response = await _menuApplication.GetAsync(id);
        return Ok(response);
    }

    [HttpPut("UpdateAsync")]
    public async Task<ActionResult<Response<bool>>> UpdateAsync([FromBody] MenuDTO menu)
    {
        var response = await _menuApplication.UpdateAsync(menu);
        return Ok(response);
    }

    [HttpDelete("DeleteAsync/{id}")]
    public async Task<ActionResult<Response<bool>>> DeleteAsync(int id)
    {
        var response = await _menuApplication.DeleteAsync(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPaginationAsync")]
    public async Task<ActionResult<ResponsePagination<IEnumerable<MenuDTO>>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = await _menuApplication.GetAllWithPaginationAsync(page, pageSize);
        return Ok(response);
    }

    [HttpGet("CountAsync")]
    public async Task<ActionResult<Response<int>>> CountAsync()
    {
        var response = await _menuApplication.CountAsync();
        return Ok(response);
    }

    #endregion
}
