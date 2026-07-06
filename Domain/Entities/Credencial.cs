namespace Domain.Entities;

public class Credencial : BaseAuditableEntity 
{
    public int IdUsuario { get; set; }
    public int IdCredencial { get; set; } // Referencia a CatCredencial

    public string Hash { get; set; } = null!;
    public string? Salt { get; set; }

    // ==========================================
    // PROPIEDADES DE NAVEGACIÓN
    // ==========================================
    public Usuario Usuario { get; set; } = null!;
    public CatCredencial CatCredencial { get; set; } = null!;
}