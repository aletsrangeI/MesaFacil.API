using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("Proveedores")]
public class Proveedor : BaseAuditableEntity
{
    public int IdEmpresa { get; set; }

    [Required]
    [MaxLength(13)]
    public string RFC { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string RazonSocial { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? NombreComercial { get; set; }

    [MaxLength(150)]
    public string? Email { get; set; }

    [MaxLength(30)]
    public string? Telefono { get; set; }

    [MaxLength(100)]
    public string? Contacto { get; set; }

    [MaxLength(300)]
    public string? Direccion { get; set; }

    /// <summary>
    /// Clave SAT del régimen fiscal (ej. 601 General de Ley Personas Morales, 612 Personas Físicas con Actividades Empresariales, 626 RESICO)
    /// </summary>
    [MaxLength(10)]
    public string? RegimenFiscal { get; set; }

    /// <summary>
    /// Días de crédito pactados. 0 = De contado.
    /// </summary>
    public int DiasCredito { get; set; } = 0;

    [MaxLength(50)]
    public string? Banco { get; set; }

    [MaxLength(30)]
    public string? CuentaBancaria { get; set; }

    [ForeignKey(nameof(IdEmpresa))]
    public virtual Empresa? Empresa { get; set; }

    public virtual ICollection<MapeoInsumoProveedor> Mapeos { get; set; } = new List<MapeoInsumoProveedor>();
    public virtual ICollection<CompraFactura> Compras { get; set; } = new List<CompraFactura>();
}
