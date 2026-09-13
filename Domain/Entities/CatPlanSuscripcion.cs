using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// Spec 021: Catálogo comercial de tiers de suscripción SaaS (Barra & Café, Restaurante Pro,
/// Multi-Sucursal). Define precios y banderas de capacidades usadas por IFeatureGateService
/// para decidir el acceso a módulos cuando FeatureGating:Enabled == true.
/// NOTA IMPORTANTE: mientras FeatureGating:Enabled esté en false (valor por defecto durante
/// demos, pilotos y Restaurantes Fundadores), estas banderas no bloquean nada.
/// </summary>
[Table("CatPlanesSuscripcion")]
public class CatPlanSuscripcion : BaseAuditableEntity
{
    [Required]
    [MaxLength(30)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecioMensualMxn { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecioAnualMxn { get; set; }

    public int MaxSucursales { get; set; } = 1;

    /// <summary>
    /// Pantallas KDS incluidas de base en el plan (sin add-ons). 0 significa "sin KDS".
    /// </summary>
    public int MaxKdsBase { get; set; }

    public bool PermiteMesas { get; set; }

    public bool PermiteSplitBill { get; set; }

    public bool PermiteRecetas { get; set; }

    public bool PermiteCfdiXml { get; set; }

    public bool PermiteCxP { get; set; }

    public virtual ICollection<EmpresaSuscripcion> Suscripciones { get; set; } = new List<EmpresaSuscripcion>();
}

public static class CodigosPlanSuscripcion
{
    public const string Tier1_Barra = "Tier1_Barra";
    public const string Tier2_Pro = "Tier2_Pro";
    public const string Tier3_Multi = "Tier3_Multi";
}
