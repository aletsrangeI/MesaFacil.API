using Common;
using DTO.CatCredencial;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CatCredencialController : Controller
{
    private readonly ICatCredencialApplication _catCredencialApplication;

    public CatCredencialController(ICatCredencialApplication catCredencialApplication)
    {
        _catCredencialApplication = catCredencialApplication;
    }

    #region Metodos sincronos

    [HttpPost("Insert")]
    public IActionResult Insert([FromBody] CatCredencialDTO catCredencial)
    {
        var response = _catCredencialApplication.Insert(catCredencial);
        return Ok(response);
    }

    [HttpGet("GetAll")]
    public IActionResult GetAll()
    {
        var response = _catCredencialApplication.GetAll();
        return Ok(response);
    }

    [HttpGet("GetById/{id}")]
    public IActionResult GetById(int id)
    {
        var response = _catCredencialApplication.Get(id);
        return Ok(response);
    }

    [HttpPut("Update")]
    public IActionResult Update([FromBody] CatCredencialDTO catCredencial)
    {
        var response = _catCredencialApplication.Update(catCredencial);
        return Ok(response);
    }

    [HttpDelete("Delete/{id}")]
    public IActionResult Delete(int id)
    {
        var response = _catCredencialApplication.Delete(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPagination")]
    public IActionResult GetAllWithPagination(int page, int pageSize)
    {
        var response = _catCredencialApplication.GetAllWithPagination(page, pageSize);
        return Ok(response);
    }

    [HttpGet("Count")]
    public IActionResult Count()
    {
        var response = _catCredencialApplication.Count();
        return Ok(response);
    }

    #endregion

    #region Metodos asincronos

    [HttpPost("InsertAsync")]
    public async Task<IActionResult> InsertAsync([FromBody] CatCredencialDTO catCredencial)
    {
        var response = await _catCredencialApplication.InsertAsync(catCredencial);
        return Ok(response);
    }

    [HttpGet("GetAllAsync")]
    public async Task<IActionResult> GetAllAsync()
    {
        var response = await _catCredencialApplication.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("GetByIdAsync/{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var response = await _catCredencialApplication.GetAsync(id);
        return Ok(response);
    }

    [HttpPut("UpdateAsync")]
    public async Task<IActionResult> UpdateAsync([FromBody] CatCredencialDTO catCredencial)
    {
        var response = await _catCredencialApplication.UpdateAsync(catCredencial);
        return Ok(response);
    }

    [HttpDelete("DeleteAsync/{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var response = await _catCredencialApplication.DeleteAsync(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPaginationAsync")]
    public async Task<IActionResult> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = await _catCredencialApplication.GetAllWithPaginationAsync(page, pageSize);
        return Ok(response);
    }

    [HttpGet("CountAsync")]
    public async Task<IActionResult> CountAsync()
    {
        var response = await _catCredencialApplication.CountAsync();
        return Ok(response);
    }

    #endregion
}