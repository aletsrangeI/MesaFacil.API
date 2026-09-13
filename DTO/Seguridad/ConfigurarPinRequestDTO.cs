namespace DTO.Seguridad;

/// <summary>
/// Spec 024: asignación/cambio del PIN de supervisor de 4 dígitos.
/// Si IdUsuario es null, se asume el usuario autenticado (JWT "sub").
/// </summary>
public class ConfigurarPinRequestDTO
{
    public int? IdUsuario { get; set; }
    public string NuevoPin { get; set; } = string.Empty;
}
