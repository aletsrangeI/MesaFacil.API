using Common;
using DTO.TicketDetalle;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TicketDetallesController : ControllerBase
{
    private readonly ITicketDetalleApplication _ticketDetalleApplication;

    public TicketDetallesController(ITicketDetalleApplication ticketDetalleApplication)
    {
        _ticketDetalleApplication = ticketDetalleApplication;
    }

    #region Metodos sincronos

    [HttpPost("Insert")]
    public ActionResult<Response<bool>> Insert([FromBody] TicketDetalleDTO dto)
    {
        var response = _ticketDetalleApplication.Insert(dto);
        return Ok(response);
    }

    [HttpGet("GetAll")]
    public ActionResult<Response<IEnumerable<TicketDetalleDTO>>> GetAll()
    {
        var response = _ticketDetalleApplication.GetAll();
        return Ok(response);
    }

    [HttpGet("GetById/{id}")]
    public ActionResult<Response<TicketDetalleDTO>> GetById(int id)
    {
        var response = _ticketDetalleApplication.Get(id);
        return Ok(response);
    }

    [HttpPut("Update")]
    public ActionResult<Response<bool>> Update([FromBody] TicketDetalleDTO dto)
    {
        var response = _ticketDetalleApplication.Update(dto);
        return Ok(response);
    }

    [HttpDelete("Delete/{id}")]
    public ActionResult<Response<bool>> Delete(int id)
    {
        var response = _ticketDetalleApplication.Delete(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPagination")]
    public ActionResult<ResponsePagination<IEnumerable<TicketDetalleDTO>>> GetAllWithPagination(int page, int pageSize)
    {
        var response = _ticketDetalleApplication.GetAllWithPagination(page, pageSize);
        return Ok(response);
    }

    [HttpGet("Count")]
    public ActionResult<Response<int>> Count()
    {
        var response = _ticketDetalleApplication.Count();
        return Ok(response);
    }

    #endregion

    #region Metodos asincronos

    [HttpPost("InsertAsync")]
    public async Task<ActionResult<Response<bool>>> InsertAsync([FromBody] TicketDetalleDTO dto)
    {
        var response = await _ticketDetalleApplication.InsertAsync(dto);
        return Ok(response);
    }

    [HttpGet("GetAllAsync")]
    public async Task<ActionResult<Response<IEnumerable<TicketDetalleDTO>>>> GetAllAsync()
    {
        var response = await _ticketDetalleApplication.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("GetByIdAsync/{id}")]
    public async Task<ActionResult<Response<TicketDetalleDTO>>> GetByIdAsync(int id)
    {
        var response = await _ticketDetalleApplication.GetAsync(id);
        return Ok(response);
    }

    [HttpPut("UpdateAsync")]
    public async Task<ActionResult<Response<bool>>> UpdateAsync([FromBody] TicketDetalleDTO dto)
    {
        var response = await _ticketDetalleApplication.UpdateAsync(dto);
        return Ok(response);
    }

    [HttpDelete("DeleteAsync/{id}")]
    public async Task<ActionResult<Response<bool>>> DeleteAsync(int id)
    {
        var response = await _ticketDetalleApplication.DeleteAsync(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPaginationAsync")]
    public async Task<ActionResult<ResponsePagination<IEnumerable<TicketDetalleDTO>>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = await _ticketDetalleApplication.GetAllWithPaginationAsync(page, pageSize);
        return Ok(response);
    }

    [HttpGet("CountAsync")]
    public async Task<ActionResult<Response<int>>> CountAsync()
    {
        var response = await _ticketDetalleApplication.CountAsync();
        return Ok(response);
    }

    #endregion
}
