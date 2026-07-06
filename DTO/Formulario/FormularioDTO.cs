using DTO.FormField;
using System.Collections.Generic;

namespace DTO.Formulario;

public class FormularioDTO
{
    public int Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    
    public List<FormFieldDTO> Campos { get; set; } = new();
}