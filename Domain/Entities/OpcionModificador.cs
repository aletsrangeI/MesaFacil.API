namespace Domain.Entities;

public class OpcionModificador : BaseAuditableEntity
{
    public int IdGrupo { get; set; }
    public string? Nombre { get; set; }
    public decimal PrecioExtra { get; set; }
    public bool EsDefault { get; set; }

    public GrupoModificador Grupo { get; set; } = null!;
    public ICollection<PedidoModificador> PedidoModificadores { get; set; } = new List<PedidoModificador>();
}