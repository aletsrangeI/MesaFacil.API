using Domain.Entities;
using Interface.Suscripciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence.Context;

namespace UseCases.Suscripciones;

/// <summary>
/// Spec 021: implementación por defecto de IFeatureGateService.
///
/// Regla de oro: si "FeatureGating:Enabled" no está en true explícitamente, se concede acceso
/// total sin tocar la base de datos (modo demo / Restaurantes Fundadores). Esto incluye el caso
/// en que la sección de configuración no exista en absoluto.
/// </summary>
public class FeatureGateService : IFeatureGateService
{
    /// <summary>
    /// Periodo de gracia operativo tras el vencimiento de una suscripción: nunca se corta el
    /// servicio a mitad de un turno, se dan 5 días hábiles de cortesía antes de bloquear.
    /// </summary>
    public const int DiasGraciaHabiles = 5;

    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _configuration;

    public FeatureGateService(ApplicationDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task<bool> TieneAccesoAsync(int empresaId, string featureName)
    {
        if (!EstaHabilitadoElGating())
            return true;

        var suscripcion = await _db.EmpresasSuscripcion
            .Include(s => s.Plan)
            .Where(s => s.IdEmpresa == empresaId)
            .OrderByDescending(s => s.FechaInicio)
            .FirstOrDefaultAsync();

        if (suscripcion == null || suscripcion.Plan == null)
        {
            // Sin suscripción asignada y el gating está activo: se niega el acceso a
            // características de pago (comportamiento explícito, no accidental).
            return false;
        }

        if (!VigenciaEfectiva(suscripcion))
            return false;

        return EvaluarCapacidadDelPlan(suscripcion, featureName);
    }

    private bool EstaHabilitadoElGating()
    {
        return _configuration.GetValue<bool>("FeatureGating:Enabled", false);
    }

    /// <summary>
    /// Determina si la suscripción sigue operando: vigente por fecha, o vencida pero dentro
    /// del periodo de gracia de 5 días hábiles (Criterio de Aceptación #4: "Cero Bloqueos
    /// Operativos").
    /// </summary>
    private static bool VigenciaEfectiva(EmpresaSuscripcion suscripcion)
    {
        var ahora = DateTime.UtcNow;

        if (suscripcion.FechaFinVigencia >= ahora)
            return true;

        if (!suscripcion.EnPeriodoGracia)
            return false;

        var diasHabilesVencido = ContarDiasHabiles(suscripcion.FechaFinVigencia, ahora);
        return diasHabilesVencido <= DiasGraciaHabiles;
    }

    private static int ContarDiasHabiles(DateTime desde, DateTime hasta)
    {
        if (hasta <= desde) return 0;

        int diasHabiles = 0;
        var cursor = desde.Date;
        var limite = hasta.Date;

        while (cursor < limite)
        {
            cursor = cursor.AddDays(1);
            if (cursor.DayOfWeek != DayOfWeek.Saturday && cursor.DayOfWeek != DayOfWeek.Sunday)
                diasHabiles++;
        }

        return diasHabiles;
    }

    private static bool EvaluarCapacidadDelPlan(EmpresaSuscripcion suscripcion, string featureName)
    {
        var plan = suscripcion.Plan!;

        return featureName switch
        {
            FeatureNames.ModuloMesas => plan.PermiteMesas,
            FeatureNames.ModuloSplitBill => plan.PermiteSplitBill,
            FeatureNames.ModuloRecetas => plan.PermiteRecetas,
            FeatureNames.ModuloCfdiXml => plan.PermiteCfdiXml,
            FeatureNames.ModuloCxP => plan.PermiteCxP,
            FeatureNames.ModuloKds => (plan.MaxKdsBase + suscripcion.KdsAddonsContratados) > 0,
            // Feature desconocida: no se bloquea por default para no romper módulos que
            // todavía no han sido mapeados explícitamente a una capacidad del plan.
            _ => true
        };
    }
}

/// <summary>
/// Nombres de feature reconocidos por [RequireFeature] / IFeatureGateService.
/// </summary>
public static class FeatureNames
{
    public const string ModuloMesas = "ModuloMesas";
    public const string ModuloSplitBill = "ModuloSplitBill";
    public const string ModuloRecetas = "ModuloRecetas";
    public const string ModuloCfdiXml = "ModuloCfdiXml";
    public const string ModuloCxP = "ModuloCxP";
    public const string ModuloKds = "ModuloKds";
}
