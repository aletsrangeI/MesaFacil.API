using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// Spec 020: Saldo de timbres fiscales (CFDI) disponible por Empresa.
/// Cada timbrado exitoso decrementa 1 unidad de TimbresDisponibles e incrementa TimbresConsumidos.
/// </summary>
[Table("EmpresaBolsasTimbres")]
public class EmpresaBolsaTimbres : BaseAuditableEntity
{
    public int IdEmpresa { get; set; }

    public int TimbresDisponibles { get; set; }

    public int TimbresConsumidos { get; set; }

    public DateTime? UltimaRecargaFecha { get; set; }

    [ForeignKey(nameof(IdEmpresa))]
    public virtual Empresa? Empresa { get; set; }

    public virtual ICollection<ConsumoTimbreHistorial> Historial { get; set; } = new List<ConsumoTimbreHistorial>();
}
