using Common;
using Domain.Entities;
using DTO.Mesa;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApi.Hubs;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class MesasController : ControllerBase
{
    private readonly IMesaApplication _mesaApplication;
    private readonly IHubContext<MesasHub>? _mesasHub;

    public MesasController(IMesaApplication mesaApplication, IHubContext<MesasHub>? mesasHub = null)
    {
        _mesaApplication = mesaApplication;
        _mesasHub = mesasHub;
    }

    #region Metodos sincronos

    [HttpPost("Insert")]
    public ActionResult<Response<bool>> Insert([FromBody] MesaDTO mesa)
    {
        var response = _mesaApplication.Insert(mesa);
        return Ok(response);
    }

    [HttpGet("GetAll")]
    public ActionResult<Response<IEnumerable<MesaDTO>>> GetAll()
    {
        var response = _mesaApplication.GetAll();
        return Ok(response);
    }

    [HttpGet("GetById/{id}")]
    public ActionResult<Response<MesaDTO>> GetById(int id)
    {
        var response = _mesaApplication.Get(id);
        return Ok(response);
    }

    [HttpPut("Update")]
    public ActionResult<Response<bool>> Update([FromBody] MesaDTO mesa)
    {
        var response = _mesaApplication.Update(mesa);
        if (response.isSuccess && _mesasHub != null)
        {
            _ = _mesasHub.Clients.All.SendAsync("MesaEstadoActualizado", new
            {
                idMesa = mesa.Id,
                idEstadoMesa = mesa.IdEstadoMesa,
                codigoMesa = mesa.Codigo,
                idSucursal = mesa.IdSucursal,
                timestamp = DateTime.UtcNow
            });
        }
        return Ok(response);
    }

    [HttpDelete("Delete/{id}")]
    public ActionResult<Response<bool>> Delete(int id)
    {
        var response = _mesaApplication.Delete(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPagination")]
    public ActionResult<ResponsePagination<IEnumerable<MesaDTO>>> GetAllWithPagination(int page, int pageSize)
    {
        var response = _mesaApplication.GetAllWithPagination(page, pageSize);
        return Ok(response);
    }

    [HttpGet("Count")]
    public ActionResult<Response<int>> Count()
    {
        var response = _mesaApplication.Count();
        return Ok(response);
    }

    #endregion

    #region Metodos asincronos

    [HttpPost("InsertAsync")]
    public async Task<ActionResult<Response<bool>>> InsertAsync([FromBody] MesaDTO mesa)
    {
        var response = await _mesaApplication.InsertAsync(mesa);
        return Ok(response);
    }

    [HttpGet("GetAllAsync")]
    public async Task<ActionResult<Response<IEnumerable<MesaDTO>>>> GetAllAsync()
    {
        var response = await _mesaApplication.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("GetByIdAsync/{id}")]
    public async Task<ActionResult<Response<MesaDTO>>> GetByIdAsync(int id)
    {
        var response = await _mesaApplication.GetAsync(id);
        return Ok(response);
    }

    [HttpPut("UpdateAsync")]
    public async Task<ActionResult<Response<bool>>> UpdateAsync([FromBody] MesaDTO mesa)
    {
        var response = await _mesaApplication.UpdateAsync(mesa);
        if (response.isSuccess && _mesasHub != null)
        {
            await _mesasHub.Clients.All.SendAsync("MesaEstadoActualizado", new
            {
                idMesa = mesa.Id,
                idEstadoMesa = mesa.IdEstadoMesa,
                codigoMesa = mesa.Codigo,
                idSucursal = mesa.IdSucursal,
                timestamp = DateTime.UtcNow
            });
        }
        return Ok(response);
    }

    [HttpPut("{id}/solicitar-cuenta")]
    [HttpPut("SolicitarCuenta/{id}")]
    public async Task<ActionResult<Response<bool>>> SolicitarCuenta(int id)
    {
        var response = await _mesaApplication.SolicitarCuentaAsync(id);
        if (response.isSuccess && _mesasHub != null)
        {
            var mesaRes = await _mesaApplication.GetAsync(id);
            var codigoMesa = mesaRes.Data?.Codigo ?? $"M{id}";
            var idSucursal = mesaRes.Data?.IdSucursal ?? 1;

            await _mesasHub.Clients.All.SendAsync("MesaEstadoActualizado", new
            {
                idMesa = id,
                idEstadoMesa = EstadosMesaConst.PidiendoCuenta,
                codigoMesa = codigoMesa,
                idSucursal = idSucursal,
                timestamp = DateTime.UtcNow
            });
        }
        return Ok(response);
    }

    [HttpDelete("DeleteAsync/{id}")]
    public async Task<ActionResult<Response<bool>>> DeleteAsync(int id)
    {
        var response = await _mesaApplication.DeleteAsync(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPaginationAsync")]
    public async Task<ActionResult<ResponsePagination<IEnumerable<MesaDTO>>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = await _mesaApplication.GetAllWithPaginationAsync(page, pageSize);
        return Ok(response);
    }

    [HttpGet("CountAsync")]
    public async Task<ActionResult<Response<int>>> CountAsync()
    {
        var response = await _mesaApplication.CountAsync();
        return Ok(response);
    }

    [HttpPost("Unir")]
    public async Task<ActionResult<Response<bool>>> UnirMesas([FromBody] UnirMesasDTO dto)
    {
        var response = await _mesaApplication.UnirMesasAsync(dto);
        if (response.isSuccess && _mesasHub != null)
        {
            await _mesasHub.Clients.All.SendAsync("MesasUnidasActualizadas", new
            {
                idMesaPrincipal = dto.IdMesaPrincipal,
                idsMesasSecundarias = dto.IdsMesasSecundarias,
                timestamp = DateTime.UtcNow
            });
        }
        return Ok(response);
    }

    [HttpPost("Desunir/{idMesa}")]
    public async Task<ActionResult<Response<bool>>> DesunirMesa(int idMesa)
    {
        var response = await _mesaApplication.DesunirMesaAsync(idMesa);
        if (response.isSuccess && _mesasHub != null)
        {
            await _mesasHub.Clients.All.SendAsync("MesasUnidasActualizadas", new
            {
                idMesa = idMesa,
                desunida = true,
                timestamp = DateTime.UtcNow
            });
        }
        return Ok(response);
    }

    [HttpPost("DesunirGrupo/{idMesaPrincipal}")]
    public async Task<ActionResult<Response<bool>>> DesunirGrupo(int idMesaPrincipal)
    {
        var response = await _mesaApplication.DesunirGrupoAsync(idMesaPrincipal);
        if (response.isSuccess && _mesasHub != null)
        {
            await _mesasHub.Clients.All.SendAsync("MesasUnidasActualizadas", new
            {
                idMesaPrincipal = idMesaPrincipal,
                desunida = true,
                timestamp = DateTime.UtcNow
            });
        }
        return Ok(response);
    }

    #endregion
}
