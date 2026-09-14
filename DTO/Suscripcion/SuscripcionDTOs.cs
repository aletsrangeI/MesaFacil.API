namespace DTO.Suscripcion;

/// <summary>
/// Spec 021: representación pública del catálogo comercial de tiers, usada para la
/// tabla de precios / pantalla de upgrade.
/// </summary>
public class CatPlanSuscripcionDTO
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public decimal PrecioMensualMxn { get; set; }
    public decimal PrecioAnualMxn { get; set; }
    public int MaxSucursales { get; set; }
    public int MaxKdsBase { get; set; }
    public bool PermiteMesas { get; set; }
    public bool PermiteSplitBill { get; set; }
    public bool PermiteRecetas { get; set; }
    public bool PermiteCfdiXml { get; set; }
    public bool PermiteCxP { get; set; }
}

/// <summary>
/// Spec 021: suscripción vigente (o histórica más reciente) de una Empresa.
/// </summary>
public class EmpresaSuscripcionDTO
{
    public int Id { get; set; }
    public int IdEmpresa { get; set; }
    public int IdPlan { get; set; }
    public string PlanCodigo { get; set; } = string.Empty;
    public string PlanNombre { get; set; } = string.Empty;
    public bool EsPagoAnual { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFinVigencia { get; set; }
    public string EstadoSuscripcion { get; set; } = string.Empty;
    public int KdsAddonsContratados { get; set; }
    public int ComanderosAddons { get; set; }
    public bool EnPeriodoGracia { get; set; }
    public int MaxKdsPermitidos { get; set; }
}

/// <summary>
/// Spec 021: request del endpoint administrativo para asignar/cambiar el plan de una Empresa.
/// </summary>
public class AsignarPlanSuscripcionRequestDTO
{
    public int IdPlan { get; set; }
    public bool EsPagoAnual { get; set; }
    public int KdsAddonsContratados { get; set; }
    public int ComanderosAddons { get; set; }
}
