namespace DTO.Impresoras;

/// <summary>
/// Spec 023, sección 3 del spec.md: forma EXACTA de la respuesta de
/// POST /api/impresoras/test-print/{idImpresora}.
/// </summary>
public class TestPrintResponseDTO
{
    public bool Exitoso { get; set; }
    public string Mensaje { get; set; } = string.Empty;
}
