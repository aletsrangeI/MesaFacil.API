namespace DTO.Rol;

public class RolDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public bool IsSystem { get; set; }
    public bool IsAssignable { get; set; }
    public string? ConcurrencyStamp { get; set; }
}
