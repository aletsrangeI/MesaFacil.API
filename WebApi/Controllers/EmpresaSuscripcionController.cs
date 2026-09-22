using Common;
using Domain.Entities;
using DTO.Suscripcion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace WebApi.Controllers;

/// <summary>
/// Spec 021: suscripción de la Empresa a un plan (tier). Nótese que estos endpoints solo
/// administran el CATÁLOGO/asignación de plan; no bloquean nada por sí mismos — el bloqueo
/// real (si algún día se activa) vive en [RequireFeature] / IFeatureGateService, gobernado por
/// FeatureGating:Enabled (false por defecto).
/// </summary>
[Authorize]
[Route("api/empresa-suscripcion")]
[ApiController]
public class EmpresaSuscripcionController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EmpresaSuscripcionController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("mi-suscripcion")]
    public async Task<ActionResult<Response<EmpresaSuscripcionDTO>>> GetMiSuscripcion()
    {
        var empresaIdClaim = User.FindFirst("empresa_id")?.Value;
        if (!int.TryParse(empresaIdClaim, out var empresaId))
        {
            return Unauthorized(new Response<EmpresaSuscripcionDTO>
            {
                isSuccess = false,
                Message = "No se pudo determinar la empresa del usuario autenticado."
            });
        }

        var suscripcion = await _context.EmpresasSuscripcion
            .Include(s => s.Plan)
            .Where(s => s.IdEmpresa == empresaId)
            .OrderByDescending(s => s.FechaInicio)
            .FirstOrDefaultAsync();

        if (suscripcion == null || suscripcion.Plan == null)
        {
            return NotFound(new Response<EmpresaSuscripcionDTO>
            {
                isSuccess = false,
                Message = "La empresa no tiene una suscripción asignada todavía."
            });
        }

        return Ok(new Response<EmpresaSuscripcionDTO>
        {
            Data = MapToDTO(suscripcion),
            isSuccess = true,
            Message = "Suscripción obtenida con éxito."
        });
    }

    /// <summary>
    /// Endpoint administrativo simple para asignar/cambiar el plan de una empresa.
    /// TODO (fuera de alcance de esta iteración de baja prioridad): restringir por rol Admin.
    /// </summary>
    [HttpPut("{empresaId:int}")]
    public async Task<ActionResult<Response<EmpresaSuscripcionDTO>>> AsignarPlan(int empresaId, [FromBody] AsignarPlanSuscripcionRequestDTO dto)
    {
        var plan = await _context.CatPlanesSuscripcion.FirstOrDefaultAsync(p => p.Id == dto.IdPlan && p.IsActive);
        if (plan == null)
        {
            return BadRequest(new Response<EmpresaSuscripcionDTO>
            {
                isSuccess = false,
                Message = "El plan especificado no existe o no está activo."
            });
        }

        var empresaExiste = await _context.Empresas.AnyAsync(e => e.Id == empresaId);
        if (!empresaExiste)
        {
            return NotFound(new Response<EmpresaSuscripcionDTO>
            {
                isSuccess = false,
                Message = "La empresa especificada no existe."
            });
        }

        var suscripcion = await _context.EmpresasSuscripcion.FirstOrDefaultAsync(s => s.IdEmpresa == empresaId);
        var ahora = DateTime.UtcNow;
        var fechaFin = dto.EsPagoAnual ? ahora.AddYears(1) : ahora.AddMonths(1);

        if (suscripcion == null)
        {
            suscripcion = new EmpresaSuscripcion
            {
                IdEmpresa = empresaId,
                IsActive = true
            };
            _context.EmpresasSuscripcion.Add(suscripcion);
        }

        suscripcion.IdPlan = plan.Id;
        suscripcion.EsPagoAnual = dto.EsPagoAnual;
        suscripcion.FechaInicio = ahora;
        suscripcion.FechaFinVigencia = fechaFin;
        suscripcion.EstadoSuscripcion = EstadoSuscripcionValores.Activa;
        suscripcion.KdsAddonsContratados = dto.KdsAddonsContratados;
        suscripcion.ComanderosAddons = dto.ComanderosAddons;
        suscripcion.EnPeriodoGracia = false;

        await _context.SaveChangesAsync();

        var suscripcionCompleta = await _context.EmpresasSuscripcion
            .Include(s => s.Plan)
            .FirstAsync(s => s.Id == suscripcion.Id);

        return Ok(new Response<EmpresaSuscripcionDTO>
        {
            Data = MapToDTO(suscripcionCompleta),
            isSuccess = true,
            Message = "Plan de suscripción asignado con éxito."
        });
    }

    private static EmpresaSuscripcionDTO MapToDTO(EmpresaSuscripcion s)
    {
        return new EmpresaSuscripcionDTO
        {
            Id = s.Id,
            IdEmpresa = s.IdEmpresa,
            IdPlan = s.IdPlan,
            PlanCodigo = s.Plan?.Codigo ?? string.Empty,
            PlanNombre = s.Plan?.Nombre ?? string.Empty,
            EsPagoAnual = s.EsPagoAnual,
            FechaInicio = s.FechaInicio,
            FechaFinVigencia = s.FechaFinVigencia,
            EstadoSuscripcion = s.EstadoSuscripcion,
            KdsAddonsContratados = s.KdsAddonsContratados,
            ComanderosAddons = s.ComanderosAddons,
            EnPeriodoGracia = s.EnPeriodoGracia,
            MaxKdsPermitidos = (s.Plan?.MaxKdsBase ?? 0) + s.KdsAddonsContratados,
            MotivoSuspension = s.MotivoSuspension,
            ContactoWhatsApp = s.ContactoWhatsApp
        };
    }
}
