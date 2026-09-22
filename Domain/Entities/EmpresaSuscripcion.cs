using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// Spec 021: Suscripción activa de una Empresa a un CatPlanSuscripcion. IdEmpresa sigue
/// siendo int (no forma parte de las entidades operativas migradas a Guid en Spec 019).
/// Cuando FeatureGating:Enabled == false, IFeatureGateService ignora por completo este
/// registro y concede acceso total (modo demo / Restaurantes Fundadores).
/// </summary>
[Table("EmpresasSuscripcion")]
public class EmpresaSuscripcion : BaseAuditableEntity
{
    public int IdEmpresa { get; set; }

    public int IdPlan { get; set; }

    public bool EsPagoAnual { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFinVigencia { get; set; }

    /// <summary>
    /// Activa | EnGracia | Suspendida
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string EstadoSuscripcion { get; set; } = EstadoSuscripcionValores.Activa;

    /// <summary>
    /// Pantallas KDS adicionales contratadas como add-on (+$299/mes c/u), sumadas a
    /// CatPlanSuscripcion.MaxKdsBase.
    /// </summary>
    public int KdsAddonsContratados { get; set; }

    /// <summary>
    /// Comanderos móviles adicionales contratados como add-on (+$199/mes c/u).
    /// </summary>
    public int ComanderosAddons { get; set; }

    /// <summary>
    /// Indica si la suscripción vencida está actualmente dentro del periodo de gracia
    /// operativo (5 días hábiles) durante el cual nunca se bloquea el servicio.
    /// </summary>
    public bool EnPeriodoGracia { get; set; }

    /// <summary>
    /// Spec 033: Motivo por el cual la empresa fue suspendida desde OrionSys Central Hub o sistema de cobranza.
    /// </summary>
    [MaxLength(250)]
    public string? MotivoSuspension { get; set; }

    /// <summary>
    /// Spec 033: Número o enlace de WhatsApp de soporte/cobranza personalizado para mostrar en la pantalla de suspensión.
    /// </summary>
    [MaxLength(50)]
    public string? ContactoWhatsApp { get; set; }

    /// <summary>
    /// Spec 033: Fecha UTC del último webhook/evento recibido desde OrionSys Central Hub.
    /// </summary>
    public DateTime? UltimaActualizacionHub { get; set; }

    [ForeignKey(nameof(IdEmpresa))]
    public virtual Empresa? Empresa { get; set; }

    [ForeignKey(nameof(IdPlan))]
    public virtual CatPlanSuscripcion? Plan { get; set; }
}

public static class EstadoSuscripcionValores
{
    public const string Activa = "Activa";
    public const string EnGracia = "EnGracia";
    public const string Suspendida = "Suspendida";
}
