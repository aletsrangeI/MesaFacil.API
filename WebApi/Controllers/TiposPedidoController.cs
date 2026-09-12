using DTO.TiposPedido;
using Interface.UseCases.TiposPedido;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TiposPedidoController : ControllerBase
{
    private readonly ITipoPedidoApplication _application;

    public TiposPedidoController(ITipoPedidoApplication application)
    {
        _application = application;
    }

    [HttpPost]
    public async Task<IActionResult> Insert([FromBody] TipoPedidoDTO dto)
    {
        var response = await _application.InsertAsync(dto);
        if (response.isSuccess) return Ok(response);
        return BadRequest(response);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] TipoPedidoDTO dto)
    {
        var response = await _application.UpdateAsync(dto);
        if (response.isSuccess) return Ok(response);
        return BadRequest(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _application.DeleteAsync(id);
        if (response.isSuccess) return Ok(response);
        return BadRequest(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var response = await _application.GetAsync(id);
        if (response.isSuccess) return Ok(response);
        return BadRequest(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _application.GetAllAsync();
        if (response.isSuccess) return Ok(response);
        return BadRequest(response);
    }

    [HttpGet("pagination")]
    public async Task<IActionResult> GetAllWithPagination([FromQuery] int page, [FromQuery] int pageSize)
    {
        var response = await _application.GetAllWithPaginationAsync(page, pageSize);
        if (response.isSuccess) return Ok(response);
        return BadRequest(response);
    }

    [HttpGet("count")]
    public async Task<IActionResult> Count()
    {
        var response = await _application.CountAsync();
        if (response.isSuccess) return Ok(response);
        return BadRequest(response);
    }
}
