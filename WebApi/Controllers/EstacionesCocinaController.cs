using Common;
using DTO.EstacionCocina;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[Route("api/estaciones-cocina")]
[ApiController]
public class EstacionesCocinaController : ControllerBase
{
    private readonly IEstacionCocinaApplication _estacionApplication;

    public EstacionesCocinaController(IEstacionCocinaApplication estacionApplication)
    {
        _estacionApplication = estacionApplication;
    }

    #region Metodos sincronos

    [HttpPost("Insert")]
    public ActionResult<Response<bool>> Insert([FromBody] EstacionCocinaDTO dto)
    {
        var response = _estacionApplication.Insert(dto);
        return Ok(response);
    }

    [HttpGet("GetAll")]
    public ActionResult<Response<IEnumerable<EstacionCocinaDTO>>> GetAll()
    {
        var response = _estacionApplication.GetAll();
        return Ok(response);
    }

    [HttpGet("GetById/{id}")]
    public ActionResult<Response<EstacionCocinaDTO>> GetById(int id)
    {
        var response = _estacionApplication.Get(id);
        return Ok(response);
    }

    [HttpPut("Update")]
    public ActionResult<Response<bool>> Update([FromBody] EstacionCocinaDTO dto)
    {
        var response = _estacionApplication.Update(dto);
        return Ok(response);
    }

    [HttpDelete("Delete/{id}")]
    public ActionResult<Response<bool>> Delete(int id)
    {
        var response = _estacionApplication.Delete(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPagination")]
    public ActionResult<ResponsePagination<IEnumerable<EstacionCocinaDTO>>> GetAllWithPagination(int page, int pageSize)
    {
        var response = _estacionApplication.GetAllWithPagination(page, pageSize);
        return Ok(response);
    }

    [HttpGet("Count")]
    public ActionResult<Response<int>> Count()
    {
        var response = _estacionApplication.Count();
        return Ok(response);
    }

    #endregion

    #region Metodos asincronos

    [HttpPost("InsertAsync")]
    public async Task<ActionResult<Response<bool>>> InsertAsync([FromBody] EstacionCocinaDTO dto)
    {
        var response = await _estacionApplication.InsertAsync(dto);
        return Ok(response);
    }

    [HttpGet("GetAllAsync")]
    public async Task<ActionResult<Response<IEnumerable<EstacionCocinaDTO>>>> GetAllAsync()
    {
        var response = await _estacionApplication.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("GetByIdAsync/{id}")]
    public async Task<ActionResult<Response<EstacionCocinaDTO>>> GetByIdAsync(int id)
    {
        var response = await _estacionApplication.GetAsync(id);
        return Ok(response);
    }

    [HttpPut("UpdateAsync")]
    public async Task<ActionResult<Response<bool>>> UpdateAsync([FromBody] EstacionCocinaDTO dto)
    {
        var response = await _estacionApplication.UpdateAsync(dto);
        return Ok(response);
    }

    [HttpDelete("DeleteAsync/{id}")]
    public async Task<ActionResult<Response<bool>>> DeleteAsync(int id)
    {
        var response = await _estacionApplication.DeleteAsync(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPaginationAsync")]
    public async Task<ActionResult<ResponsePagination<IEnumerable<EstacionCocinaDTO>>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = await _estacionApplication.GetAllWithPaginationAsync(page, pageSize);
        return Ok(response);
    }

    [HttpGet("CountAsync")]
    public async Task<ActionResult<Response<int>>> CountAsync()
    {
        var response = await _estacionApplication.CountAsync();
        return Ok(response);
    }

    #endregion
}
